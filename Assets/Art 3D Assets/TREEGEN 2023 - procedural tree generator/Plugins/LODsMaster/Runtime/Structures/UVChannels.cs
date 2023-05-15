using System.Runtime.CompilerServices;

namespace UnityMeshSimplifier
{
    internal class UVChannels<TVec>
    {
        private static readonly int UVChannelCount = MeshUtils.UVChannelCount;

        private ResizableArray<TVec>[] channels = null;
        private TVec[][] channelsData = null;

        public TVec[][] Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                for (int i = 0; i < UVChannelCount; i++)
                {
                    if (channels[i] != null)
                    {
                        channelsData[i] = channels[i].Data;
                    }
                    else
                    {
                        channelsData[i] = null;
                    }
                }
                return channelsData;
            }
        }

        /// <summary>
        /// Gets or sets a specific channel by index.
        /// </summary>
        /// <param name="index">The channel index.</param>
        public ResizableArray<TVec> this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return channels[index]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { channels[index] = value; }
        }

        public UVChannels()
        {
            channels = new ResizableArray<TVec>[UVChannelCount];
            channelsData = new TVec[UVChannelCount][];
        }

        /// <summary>
        /// Resizes all channels at once.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        /// <param name="trimExess">If exess memory should be trimmed.</param>
        public void Resize(int capacity, bool trimExess = false)
        {
            for (int i = 0; i < UVChannelCount; i++)
            {
                if (channels[i] != null)
                {
                    channels[i].Resize(capacity, trimExess);
                }
            }
        }
    }
}