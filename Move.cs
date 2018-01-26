using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorphAI_Delinquent {

	/// <summary>
	/// Represents and stores the details of a single game move. 
	/// </summary> 
	/// <para>
	/// Author: Robert Thompson
	/// Date: March 14, 2017
	/// </para>
	class Move {

		private int fromRow;
		private int fromCol;
		private int toRow;
		private int toCol;

		public Move(int fromRow, int fromCol, int toRow, int toCol) {
			this.fromRow = fromRow;
			this.fromCol = fromCol;
			this.toRow = toRow;
			this.toCol = toCol;
		}//end of constructor

		public Move(int[] points) {
			if(points.Length == 4) {
				this.fromRow = points[0];
				this.fromCol = points[1];
				this.toRow = points[2];
				this.toCol = points[3];
			}
			else {
				this.fromRow = -1;
				this.fromCol = -1;
				this.toRow = -1;
				this.toCol = -1;
			}
		}//end of constructor

		public int GetFromRow() {
			return fromRow;
		}//end of GetFromRow method

		public int GetFromCol() {
			return fromCol;
		}//end of GetFromCol method

		public int GetToRow() {
			return toRow;
		}//end of GetToRow method

		public int GetToCol() {
			return toCol;
		}//end of GetToCol method

		public int[] ToArray() {
			int[] returnValue = new int[4];
			returnValue[0] = fromRow;
			returnValue[1] = fromCol;
			returnValue[2] = toRow;
			returnValue[3] = toCol;
			return returnValue;
		}//end of ToArray method

		public override string ToString() {
			return $"M[{fromRow}],[{fromCol}],[{toRow}],[{toCol}]";
		}//end of ToString method

		public override bool Equals(object obj) {
			bool returnValue = false;
			if(obj is Move) {
				Move m = (Move)obj;
				if(		this.fromRow == m.fromRow &&
						this.fromCol == m.fromCol &&
						this.toRow == m.toRow &&
						this.toCol == m.toCol) {
					returnValue = true;
				}
			}
			return returnValue;
		}//end of Equals method

		public override int GetHashCode() {
			return base.GetHashCode();
		}//end of GetHasCode method

		public static bool operator ==(Move a, Move b) {
			bool returnValue = false;
			if(object.ReferenceEquals(null, a)) {
				returnValue = object.ReferenceEquals(null, b);
			}
			else if(object.ReferenceEquals(null, b)) {
				returnValue = object.ReferenceEquals(null, a);
			}
			else if(a.fromRow == b.fromRow &&
					a.fromCol == b.fromCol &&
					a.toRow == b.toRow &&
					a.toCol == b.toCol) {
				returnValue = true;
			}
			return returnValue;
		}//end of operator= method

		public static bool operator !=(Move a, Move b) {
			bool returnValue = false;
			if(object.ReferenceEquals(null, a)) {
				returnValue = !object.ReferenceEquals(null, b);
			}
			else if(object.ReferenceEquals(null, b)) {
				returnValue = !object.ReferenceEquals(null, a);
			}
			else if(a.fromRow != b.fromRow ||
					a.fromCol != b.fromCol ||
					a.toRow != b.toRow ||
					a.toCol != b.toCol) {
				returnValue = true;
			}
			return returnValue;
		}//end of operator= method

		public static bool Valid(Move move) {
			bool returnValue = true;
			if(		move.fromRow == -1 &&
					move.fromCol == -1 &&
					move.toRow == -1 &&
					move.toCol == -1) {
				returnValue = false;
			}
			return returnValue;
		}//end of Valid method

		public static Move Reverse(Move move) {
			return new Move(move.toRow, move.toCol, move.fromRow, move.fromCol);
		}//end of Reverse method

	}//end of Move class
}//end of Thompsor namespace