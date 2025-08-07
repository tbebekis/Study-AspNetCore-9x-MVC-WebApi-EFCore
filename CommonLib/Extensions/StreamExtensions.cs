namespace CommonLib
{
    static public class StreamExtensions
    {
        /* ●  copy-move content from one stream to another. NOTE: CopyTo() is added to Stream class in .Net 4 */
        /// <summary>
        /// Copies all bytes from the Source to the current Dest position.
        /// Does not reset the position of the Source or Dest after the copy operation is complete.
        /// </summary>
        public static void CopyAllTo(this Stream Source, Stream Dest, int BufferSize = 1024 * 1024)
        {
            Source.Position = 0;
            Source.CopyTo(Dest, BufferSize);
        }
        /// <summary>
        /// Writes the stream contents to a byte array, regardless of the Stream Position
        /// </summary>
        static public byte[] ToArray(this Stream Stream)
        {
            if (Stream is MemoryStream)
                return (Stream as MemoryStream).ToArray();

            using (MemoryStream MS = new MemoryStream())
            {
                CopyAllTo(Stream, MS);
                return MS.ToArray();
            }
        }
    }
}
