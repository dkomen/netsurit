using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netsurit.Data
{
    /// <summary>
    /// Represents a character in our random matrix and if it has already been used and found in a search or not
    /// I specifically chose not to use a tuple but rather this struct
    /// </summary>
    public struct MatrixCharacter: ICloneable
    {
        public byte Character { get; set; } //Dean: change set to private
        public bool AlreadyUsed { get; set; }

        public MatrixCharacter(byte character)
        {
            this.Character = character;
            this.AlreadyUsed = false;
        }

        /// <summary>
        /// Clone this struct
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            MatrixCharacter clonedCharacter = new MatrixCharacter(this.Character);
            clonedCharacter.AlreadyUsed = this.AlreadyUsed;

            return clonedCharacter;
        }
    }
}
