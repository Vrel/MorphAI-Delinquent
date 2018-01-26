# MorphAI-Delinquent

by Robert Thompson
Mar 14, 2017

Originally written for an assignment in an artificial intelligence class at Sacramento State University.

Implementation of Minimax search algorithm to play a game called "Morph."

* Requires .Net Framework 4.5.2 or later be installed in order to run. *

# Playing the Game
The game consists of a small console window with a gray background. First state whether you would like to go first. Enter “y” or “yes” to go first or “n” or “no” to go second. After you make a selection, the board is displayed. Delinquent will always be displayed at the top, with the opponent (you) at the bottom. Whoever goes first will be in white, while whoever goes second will be in green. Enter your moves by to-from spaces as show in the row and column labels (e.g. B3C3). After you make a move, Delinquent will take 5 seconds and then make its move. When someone wins, a message will be displayed in red announcing the winner. At this point, you may hit [Enter] to exit the game.

# Game Rules



# Coding and Architecture

Techniques: Minimax, Alpha-beta pruning, Iterative deepening, Transposition tables

The Eval() method determines an overall score for a board position by calculating the strength of the available moves for both Delinquent and its opponent and taking the difference between them (delinquent score - opponent score). Strength of available moves is calculated as the weighted sum of piece value, board advance, mobility, and piece threat.

Piece Value: The total value of all pieces remaining under the player's control. The piece values are listed below. They are adjusted from the traditional chess values to account for the potential value of the piece it will change to after a move (in the case of rook/bishop/knight) as well as factors such as the lack of a queen, the lack of pawn promotion, and the differences in mobility that result from movement rules and board dimensions in Morph.
	Rook = 7
	Knight = 5
	Bishop = 4
	Pawn = 1

Board Advance: A calculation of how far the player's pieces have moved from their original positions toward the other player (or to the left/right in the case of the king). It is considered advantageous to be less advanced due to the limited ability to move pieces back once they have been advanced down the board. Favoring positions that are closer to the player's own end of the board means they will maintain more options further into the course of the game.

Mobility: A relative value of how many moves are available to the player.
	Calculated as: ceiling of (total moves / number of pieces)
	
Piece Threat: The total value of the opponent pieces which the player is currently in a position to capture. These values are different than those listed previously in the piece value calculation. The king is very highly favored in this value as both threatening the opponents king and having your own king threatened are of vital significance to winning the game. This has the effect of orienting the resulting behavior around the “objective” of capturing the opponent's king while preventing them from doing the same.
	King = 10
	Rook = 5
	Knight = 5
	Bishop = 5
	Pawn = 2
	
In the current settings, the values are weighted as follows:
	Piece Value = 0.45
	Board Advance = 0.15
	Mobility = 0.15
	Piece Threat = 0.25
