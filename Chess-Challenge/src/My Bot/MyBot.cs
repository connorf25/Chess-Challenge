using System;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    // How deep to search with minmax
    private const int MAX_DEPTH = 4;

    /**
     * Main function to calculate next best move
     */
    public Move Think(Board board, Timer timer)
    {
        Move bestMove = Move.NullMove;
        // White wants to maximise score, while black wants to minimise
        float bestValue = (board.IsWhiteToMove) ? float.MinValue : float.MaxValue;

        var legalMoves = board.GetLegalMoves(capturesOnly: false);

        foreach (var move in legalMoves)
        {
            board.MakeMove(move);
            float moveValue;

            // If it's the bot's turn to play as white, then maximize; otherwise, minimize
            if (board.IsWhiteToMove)
            {
                moveValue = Minimax(board, MAX_DEPTH - 1, float.MinValue, float.MaxValue, true);
            }
            else
            {
                moveValue = Minimax(board, MAX_DEPTH - 1, float.MinValue, float.MaxValue, false);
            }

            board.UndoMove(move);

            if ((board.IsWhiteToMove && moveValue > bestValue) ||
                (!board.IsWhiteToMove && moveValue < bestValue))
            {
                bestValue = moveValue;
                bestMove = move;
            }
        }

        return bestMove;
    }

    /**
     * Minimax search tree function
     */
    float Minimax(Board board, int depth, float alpha, float beta, bool isMaximizing)
    {
        if (depth == 0)
        {
            return EvaluateBoard(board);
        }

        if (isMaximizing)
        {
            float maxEval = float.MinValue;
            var legalMoves = board.GetLegalMoves();
            foreach (var move in legalMoves)
            {
                board.MakeMove(move);
                float eval = Minimax(board, depth - 1, alpha, beta, !isMaximizing);
                board.UndoMove(move);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return maxEval;
        }
        else
        {
            float minEval = float.MaxValue;
            var legalMoves = board.GetLegalMoves();
            foreach (var move in legalMoves)
            {
                board.MakeMove(move);
                float eval = Minimax(board, depth - 1, alpha, beta, !isMaximizing);
                board.UndoMove(move);
                if (eval < minEval)
                {
                    minEval = eval;
                }
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return minEval;
        }
    }

    /**
     * Determine who the board state favours
     * This function counts pieces to determine who is winning
     * A positive value favours white, while a negative value favours black
     */
    private float EvaluateBoard(Board board)
    {
        float value = 0;

        // Check checkmate
        if (board.IsInCheckmate())
        {
            return board.IsWhiteToMove ? float.MaxValue : float.MinValue;
        }

        // Reward checks
        if (board.IsInCheck())
        {
            value += board.IsWhiteToMove ? 10 : -10;
        }

        // If no checkmate evaluate other pieces
        for (int pieceTypeInt = 1; pieceTypeInt <= 6; pieceTypeInt++)
        {
            var pieceType = (PieceType)pieceTypeInt;
            var whitePieces = board.GetPieceList(pieceType, true);
            var blackPieces = board.GetPieceList(pieceType, false);

            switch (pieceType)
            {
                case PieceType.Pawn:
                    value += (whitePieces.Count - blackPieces.Count) * 10;
                    break;
                case PieceType.Knight:
                case PieceType.Bishop:
                    value += (whitePieces.Count - blackPieces.Count) * 30;
                    break;
                case PieceType.Rook:
                    value += (whitePieces.Count - blackPieces.Count) * 50;
                    break;
                case PieceType.Queen:
                    value += (whitePieces.Count - blackPieces.Count) * 90;
                    break;
            }

        }
        return value;
    }

}