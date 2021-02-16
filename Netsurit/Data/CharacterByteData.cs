using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netsurit.Data
{
    /// <summary>
    /// Holds our words
    /// A word (originally from the input file) is converted to an array of bytes. bytes are not boxed and as such we have fast access to them.
    /// </summary>
    public struct CharacterByteData //Structs can be assumed to not be boxed, however .Net does not actually make this promise as I read a few years ago.
    {
        private int _length;
        /// <summary>
        ///  The upper bound that may be used by iterative statements
        /// </summary>
        public int Length // I could use array.length in iterations instead but I thought this was prettier. A < 0.001% overall jitter time is I suppose a meaningless win though.
        {
            get
            {
                return _length;
            } 
            private set
            {
                if (value > 0)
                {
                    _length = value;
                } else
                {
                    throw new ApplicationException("The bound of an array may not be a negative number"); //Basically enforcing a uint without its later constraints
                }
            }
        }
        
        /// <summary>
        /// The character bytes to use in a serach
        /// </summary>
        public byte[] Characters { get; private set; }// I elected to not use char[] as chars are utf-16 which may cause extra unneeded resource usage

        /// <summary>
        /// Create new struct that contains the input string in byte format
        /// </summary>
        /// <param name="word"></param>
        public CharacterByteData(String word)
        {
            _length = word.Length-1; //Basically enforcing a uint

            this.Characters = word.ToArray<char>().Select( //We cast to chars...
                c => (byte)c //... and then cast the chars to bytes
            ).ToArray<byte>();
        }

        public override String ToString()
        {
            char[] characters = this.Characters.Select(
                b =>
                
                    (char)b
                
                ).ToArray<char>();

            return new String(characters);
        }
    }
}
