using System;

namespace ndf5.Infrastructure.BTrees
{
    //Base Node of any in-memory node in a Btree
    internal abstract class BTreeNode : IDisposable
    {
        /// <summary>
        /// Gets the version of the tree this is a member of
        /// </summary>
        /// <value>The version</value>
        public BTreeVerson Version { get; }

        /// <summary>
        /// Address of the first byte of this Node
        /// </summary>
        /// <value>The location.</value>
        public long Location { get; }

        public BTreeNode(
            BTreeVerson aVersion,
            long aLocation)
        {
            Version = aVersion;
            Location = aLocation;

            //TODO: take access locks
        }

        public override string ToString()
        {
            return string.Format(
                $"[BTreeNode: {nameof(Version)}={Version}, " +
                $"{nameof(Location)}={Location}"+
                (mDisposed ? "(Disposed)]" : "]"));
        }

        #region IDisposable Support

        private bool 
            mDisposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                mDisposed = true;

                //Free up locks
            }
        }

        ~BTreeNode() {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
}

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
