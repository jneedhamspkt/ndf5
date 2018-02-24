using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
namespace ndf5.Checksums
{
    /// <summary>
    /// This Class is an adaptation of lookup3.c, by Bob Jenkins, May 2006, Public Domain.
    /// </summary>
    /// <remarks>
    /// Origonal remarks from http://www.burtleburtle.net/bob/c/lookup3.c below:
    /// 
    /// These are functions for producing 32-bit hashes for hash table lookup.
    /// hashword(), hashlittle(), hashlittle2(), hashbig(), mix(), and final()
    /// are externally useful functions.Routines to test the hash are included 
    /// if SELF_TEST is defined.You can use this free for any purpose.  It's in
    /// the public domain.It has no warranty.
    /// 
    /// You probably want to use hashlittle().  hashlittle() and hashbig()
    /// hash byte arrays.  hashlittle() is is faster than hashbig() on
    /// little-endian machines.  Intel and AMD are little-endian machines.
    /// On second thought, you probably want hashlittle2(), which is identical to
    /// hashlittle() except it returns two 32-bit hashes for the price of one.  
    /// You could implement hashbig2() if you wanted but I haven't bothered here.
    /// 
    /// 
    /// If you want to find a hash of, say, exactly 7 integers, do
    /// 
    /// a = i1; b = i2;  c = i3;
    ///   mix(a, b, c);
    ///     a += i4; b += i5; c += i6;
    ///   mix(a, b, c);
    ///     a += i7;
    ///   final(a, b, c);
    ///     then use c as the hash value.If you have a variable length array of
    /// 4-byte integers to hash, use hashword().  If you have a byte array(like
    /// a character string), use hashlittle().  If you have several byte arrays, or
    /// a mix of things, see the comments above hashlittle().  
    /// 
    /// Why is this so big?  I read 12 bytes at a time into 3 4-byte integers,
    /// then mix those integers.  This is fast (you can do a lot more thorough
    /// mixing with 12*3 instructions on 3 integers than you can with 3 instructions
    /// on 1 byte), but shoehorning those bytes into integers efficiently is messy.
    /// </remarks>
    public static class Lookup3
    {

        /// <summary>
        /// Rotate the specified Integer X by K bits .
        /// </summary>
        /// <returns>The rot.</returns>
        /// <param name="x">The value to rotate</param>
        /// <param name="k">Number of bits to rotate</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Rotate(uint x, int k) => (uint) (((x) << (k)) | ((x) >> (32 - (k))));

        /// <summary>
        /// mix -- mix 3 32-bit values reversibly.
        /// </summary>
        /// <remarks>
        /// From Bob Jenkins:
        /// 
        /// This is reversible, so any information in (a, b, c) before mix() is
        /// still in (a, b, c) after mix().
        ///
        /// If four pairs of(a, b, c) inputs are run through mix(), or through
        /// mix() in reverse, there are at least 32 bits of the output that
        /// are sometimes the same for one pair and different for another pair.
        /// This was tested for:
        /// * pairs that differed by one bit, by two bits, in any combination
        ///   of top bits of (a, b, c), or in any combination of bottom bits of
        ///   (a, b, c).
        /// * "differ" is defined as +, -, ^, or ~^.  For + and -, I transformed
        ///   the output delta to a Gray code(a^(a>>1)) so a string of 1's (as
        ///   is commonly produced by subtraction) look like a single 1-bit
        ///   difference.
        /// * the base values were pseudorandom, all zero but one bit set, or
        ///   all zero plus a counter that starts at zero.
        ///
        /// Some k values for my "a-=c; a^=rot(c,k); c+=b;" arrangement that
        /// satisfy this are
        ///     4  6  8 16 19  4
        ///     9 15  3 18 27 15
        ///    14  9  3  7 17  3
        /// Well, "9 15 3 18 27 15" didn't quite get 32 bits diffing
        /// for "differ" defined as + with a one-bit base and a two-bit delta.  I
        /// used http://burtleburtle.net/bob/hash/avalanche.html to choose 
        /// the operations, constants, and arrangements of the variables.
        ///
        /// This does not achieve avalanche.There are input bits of (a, b, c)
        /// that fail to affect some output bits of (a, b, c), especially of a.The
        /// most thoroughly mixed value is c, but it doesn't really even achieve
        /// avalanche in c.
        ///
        /// This allows some parallelism.  Read-after-writes are good at doubling
        /// the number of bits affected, so the goal of mixing pulls in the opposite
        /// direction as the goal of parallelism.  I did what I could.Rotates
        /// seem to cost as much as shifts on every machine I could lay my hands
        /// on, and rotates are much kinder to the top and bottom bits, so I used
        /// rotates.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Mix(ref uint a, ref uint b, ref uint c)
        {
            a -= c; a ^= Rotate(c, 4); c += b;
            b -= a; b ^= Rotate(a, 6); a += c;
            c -= b; c ^= Rotate(b, 8); b += a;
            a -= c; a ^= Rotate(c, 16); c += b;
            b -= a; b ^= Rotate(a, 19); a += c;
            c -= b; c ^= Rotate(b, 4); b += a;
        }

        /// <summary>
        /// Gets the Final mix of a specified a, b and c integer into c
        /// NOTE: HDF5 alway treats the uints here as little endian, so we do too
        /// </summary>
        /// <remarks>
        /// From Bob Jenkins:
        /// 
        /// final -- final mixing of 3 32-bit values (a,b,c) into c
        ///
        /// Pairs of(a, b, c) values differing in only a few bits will usually
        /// produce values of c that look totally different.This was tested for
        /// * pairs that differed by one bit, by two bits, in any combination
        ///   of top bits of (a, b, c), or in any combination of bottom bits of
        ///   (a, b, c).
        /// * "differ" is defined as +, -, ^, or ~^.  For + and -, I transformed
        ///   the output delta to a Gray code(a^(a>>1)) so a string of 1's (as
        ///   is commonly produced by subtraction) look like a single 1-bit
        ///   difference.
        /// * the base values were pseudorandom, all zero but one bit set, or
        ///   all zero plus a counter that starts at zero.
        /// 
        /// These constants passed:
        ///  14 11 25 16 4 14 24
        ///  12 14 25 16 4 14 24
        /// and these came close:
        ///   4  8 15 26 3 22 24
        ///  10  8 15 26 3 22 24
        ///  11  8 15 26 3 22 24
        /// </remarks>
        /// <returns>The final.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Final(ref uint a, ref uint b, ref uint c) 
        { 
            c ^= b; c -= Rotate(b,14); 
            a ^= c; a -= Rotate(c,11); 
            b ^= a; b -= Rotate(a,25); 
            c ^= b; c -= Rotate(b,16); 
            a ^= c; a -= Rotate(c,4);  
            b ^= a; b -= Rotate(a,14); 
            c ^= b; c -= Rotate(b,24); 
        }

        /// <summary>
        /// Computes the hash for an array of bytes.
        /// </summary>
        /// <returns>The hash.</returns>
        /// <param name="aBytes">Bytes to hash</param>
        /// <param name="initval">Initial value.</param>
        public static uint ComputeHash(
            byte[] aBytes,
            uint initval = 0)
        {
            int
                length = aBytes.Length;
            //Internal State
            uint
                a, b, c;
            /* Set up the internal state */
            a = b = c = 0xdeadbeef + ((uint)length) + initval;

            int 
                k = 0;

            while (length > 12)
            {
                a += (uint)(aBytes[k + 0]);
                a += (uint)(aBytes[k + 1] << 8);
                a += (uint)(aBytes[k + 2] << 16);
                a += (uint)(aBytes[k + 3] << 24);
                b += (uint)(aBytes[k + 4]);
                b += (uint)(aBytes[k + 5] << 8);
                b += (uint)(aBytes[k + 6] << 16);
                b += (uint)(aBytes[k + 7] << 24);
                c += (uint)(aBytes[k + 8]);
                c += (uint)(aBytes[k + 9] << 8);
                c += (uint)(aBytes[k +10] << 16);
                c += (uint)(aBytes[k +11] << 24);
                Mix(ref a, ref b, ref c);
                length -= 12;
                k += 12;
            }

            /*-------------------------------- last block: affect all 32 bits of (c) */
            switch (length)                   /* all the case statements fall through */
            {
                case 12: c += (uint)(aBytes[k + 11] << 24); goto case 11;
                case 11: c += (uint)(aBytes[k + 10] << 16); goto case 10;
                case 10: c += (uint)(aBytes[k + 9] << 8); goto case 9;
                case 9 : c += (uint)(aBytes[k + 8]); goto case 8;
                case 8 : b += (uint)(aBytes[k + 7] << 24); goto case 7;
                case 7 : b += (uint)(aBytes[k + 6] << 16); goto case 6;
                case 6 : b += (uint)(aBytes[k + 5] << 8); goto case 5;
                case 5 : b += (uint)(aBytes[k + 4]); goto case 4;
                case 4 : a += (uint)(aBytes[k + 3] << 24); goto case 3;
                case 3 : a += (uint)(aBytes[k + 2] << 16); goto case 2;
                case 2 : a += (uint)(aBytes[k + 1] << 8); goto case 1;
                case 1 : a += (uint)(aBytes[k + 0]); break;
                case 0: return c;
            }

            Final(ref a, ref b, ref c);
            return c;
        }
    }
}
