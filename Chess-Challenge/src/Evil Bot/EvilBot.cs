using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{
    // A simple bot that can spot mate in one, and always captures the most valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
    {
        private const int MAX_DEPTH = 4;

        public Move Think(Board board, Timer timer)
        {
            Move bestMove = Move.NullMove;
            // White wants to maximise score, while black wants to minimise
            int bestValue = (board.IsWhiteToMove) ? int.MinValue : int.MaxValue;

            var legalMoves = board.GetLegalMoves(capturesOnly: false);

            foreach (var move in legalMoves)
            {
                board.MakeMove(move);
                int moveValue;

                // If it's the bot's turn to play as white, then maximize; otherwise, minimize
                if (board.IsWhiteToMove)
                {
                    moveValue = Minimax(board, MAX_DEPTH - 1, int.MinValue, int.MaxValue, true);
                }
                else
                {
                    moveValue = Minimax(board, MAX_DEPTH - 1, int.MinValue, int.MaxValue, false);
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

        int Minimax(Board board, int depth, int alpha, int beta, bool isMaximizing)
        {
            if (depth == 0)
            {
                return EvaluateBoard(board);
            }

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                var legalMoves = board.GetLegalMoves();
                foreach (var move in legalMoves)
                {
                    board.MakeMove(move);
                    int eval = Minimax(board, depth - 1, alpha, beta, !isMaximizing);
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
                int minEval = int.MaxValue;
                var legalMoves = board.GetLegalMoves();
                foreach (var move in legalMoves)
                {
                    board.MakeMove(move);
                    int eval = Minimax(board, depth - 1, alpha, beta, !isMaximizing);
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
        private int EvaluateBoard(Board board)
        {
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
                }

            }
            return value;
        }
    }
}