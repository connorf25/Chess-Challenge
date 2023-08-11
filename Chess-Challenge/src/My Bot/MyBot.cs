using System;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    private const int MAX_DEPTH = 3;

    public Move Think(Board board, Timer timer)
    {
        Move bestMove = Move.NullMove;
        int bestValue = int.MinValue;

        var legalMoves = board.GetLegalMoves(capturesOnly: false);

        foreach (var move in legalMoves)
        {
            board.MakeMove(move);
            int moveValue = -Minimax(board, MAX_DEPTH - 1, int.MinValue, int.MaxValue, false);
            board.UndoMove(move);

            if (moveValue > bestValue)
            {
                bestValue = moveValue;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private int Minimax(Board board, int depth, int alpha, int beta, bool maximizingPlayer)
    {
        if (depth == 0 || board.IsInCheckmate() || board.IsInStalemate())
        {
            return EvaluateBoard(board);
        }

        var legalMoves = board.GetLegalMoves(capturesOnly: false);

        if (maximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (var move in legalMoves)
            {
                board.MakeMove(move);
                int eval = -Minimax(board, depth - 1, -beta, -alpha, false);
                board.UndoMove(move);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                    break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (var move in legalMoves)
            {
                board.MakeMove(move);
                int eval = -Minimax(board, depth - 1, -beta, -alpha, true);
                board.UndoMove(move);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                    break;
            }
            return minEval;
        }
    }

    private int EvaluateBoard(Board board)
    {
        // Count pieces to evaluate position
        int value = 0;

        for (int pieceTypeInt = 1; pieceTypeInt <= 6; pieceTypeInt++)
        {
            var pieceType = (PieceType)pieceTypeInt;
            var whitePieces = board.GetPieceList(pieceType, true);
            var blackPieces = board.GetPieceList(pieceType, false);

            switch (pieceType)
            {
                case PieceType.Pawn:
                    value += (whitePieces.Count - blackPieces.Count) * 1;
                    break;
                case PieceType.Knight:
                case PieceType.Bishop:
                    value += (whitePieces.Count - blackPieces.Count) * 3;
                    break;
                case PieceType.Rook:
                    value += (whitePieces.Count - blackPieces.Count) * 5;
                    break;
                case PieceType.Queen:
                    value += (whitePieces.Count - blackPieces.Count) * 9;
                    break;
                case PieceType.King:
                    value += (whitePieces.Count - blackPieces.Count) * 900;
                    break;
            }
        }

        return value;
    }
}
