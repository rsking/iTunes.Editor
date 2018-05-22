// -----------------------------------------------------------------------
// <copyright file="GZipMessageEncoderFactory.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Lyrics.Wikia
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.ServiceModel.Channels;

    /// <summary>
    /// This class is used to create the custom encoder (GZipMessageEncoder)
    /// </summary>
    internal sealed class GZipMessageEncoderFactory : MessageEncoderFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GZipMessageEncoderFactory"/> class.
        /// </summary>
        /// <param name="messageEncoderFactory">The message encoder factory.</param>
        /// <remarks>The GZip encoder wraps an inner encoder. We require a factory to be passed in that will create this inner encoder</remarks>
        public GZipMessageEncoderFactory(MessageEncoderFactory messageEncoderFactory)
        {
            if (messageEncoderFactory == null)
            {
                throw new ArgumentNullException(nameof(messageEncoderFactory), "A valid message encoder factory must be passed to the GZipEncoder");
            }

            this.Encoder = new GZipMessageEncoder(messageEncoderFactory.Encoder);
        }

        /// <inheritdoc />
        /// <remarks>The service framework uses this property to obtain an encoder from this encoder factory</remarks>
        public override MessageEncoder Encoder { get; }

        /// <inheritdoc />
        public override MessageVersion MessageVersion => this.Encoder.MessageVersion;

        /// <summary>
        /// This is the actual GZip encoder
        /// </summary>
        private class GZipMessageEncoder : MessageEncoder
        {
            private const string GZipContentType = "application/x-gzip";

            // This implementation wraps an inner encoder that actually converts a WCF Message
            // into textual XML, binary XML or some other format. This implementation then compresses the results.
            // The opposite happens when reading messages.
            // This member stores this inner encoder.
            private readonly MessageEncoder innerEncoder;

            /// <summary>
            /// Initializes a new instance of the <see cref="GZipMessageEncoder"/> class.
            /// </summary>
            /// <param name="messageEncoder">The message encoder.</param>
            /// <remarks>We require an inner encoder to be supplied (see comment above)</remarks>
            internal GZipMessageEncoder(MessageEncoder messageEncoder)
            {
                this.innerEncoder = messageEncoder ?? throw new ArgumentNullException(nameof(messageEncoder), "A valid message encoder must be passed to the GZipEncoder");
            }

            /// <inheritdoc />
            public override string ContentType => GZipContentType;

            /// <inheritdoc />
            public override string MediaType => GZipContentType;

            /// <inheritdoc />
            /// <remarks>We delegate to the inner encoder for this</remarks>
            public override MessageVersion MessageVersion => this.innerEncoder.MessageVersion;

            /// <inheritdoc />
            public override bool IsContentTypeSupported(string contentType) => base.IsContentTypeSupported(contentType) || this.innerEncoder.IsContentTypeSupported(contentType);

            /// <inheritdoc />
            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                // see if the buffer needs to be decoded
                if (buffer.Array[buffer.Offset] == '<')
                {
                    var returnMessage = this.innerEncoder.ReadMessage(buffer, bufferManager, contentType);
                    returnMessage.Properties.Encoder = this.innerEncoder;
                    return returnMessage;
                }
                else
                {
                    // Decompress the buffer
                    var decompressedBuffer = DecompressBuffer(buffer, bufferManager);

                    // Use the inner encoder to decode the decompressed buffer
                    var returnMessage = this.innerEncoder.ReadMessage(decompressedBuffer, bufferManager, contentType);
                    returnMessage.Properties.Encoder = this;
                    return returnMessage;
                }
            }

            /// <inheritdoc />
            public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset) => this.innerEncoder.WriteMessage(message, maxMessageSize, bufferManager, 0);

            /// <inheritdoc />
            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {
                // Pass false for the "leaveOpen" parameter to the GZipStream constructor.
                // This will ensure that the inner stream gets closed when the message gets closed, which
                // will ensure that resources are available for reuse/release.
                var gzStream = new GZipStream(stream, CompressionMode.Decompress, false);
                return this.innerEncoder.ReadMessage(gzStream, maxSizeOfHeaders);
            }

            /// <inheritdoc />
            public override void WriteMessage(Message message, Stream stream)
            {
                this.innerEncoder.WriteMessage(message, stream);
                stream.Flush();
            }

            /// <summary>
            /// Helper method to decompress an array of bytes
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            /// <param name="bufferManager">The buffer manager.</param>
            /// <returns>The decompressed buffer.</returns>
            private static ArraySegment<byte> DecompressBuffer(ArraySegment<byte> buffer, BufferManager bufferManager)
            {
                const int blockSize = 1024;

                var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count);
                var decompressedStream = new MemoryStream();
                var totalRead = 0;
                var tempBuffer = bufferManager.TakeBuffer(blockSize);

                using (var decompression = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    while (true)
                    {
                        int bytesRead = decompression.Read(tempBuffer, 0, blockSize);
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        decompressedStream.Write(tempBuffer, 0, bytesRead);
                        totalRead += bytesRead;
                    }
                }

                bufferManager.ReturnBuffer(tempBuffer);

                var decompressedBytes = decompressedStream.ToArray();
                var bufferManagerBuffer = bufferManager.TakeBuffer(decompressedBytes.Length + buffer.Offset);
                Array.Copy(buffer.Array, 0, bufferManagerBuffer, 0, buffer.Offset);
                Array.Copy(decompressedBytes, 0, bufferManagerBuffer, buffer.Offset, decompressedBytes.Length);

                var byteArray = new ArraySegment<byte>(bufferManagerBuffer, buffer.Offset, decompressedBytes.Length);
                bufferManager.ReturnBuffer(buffer.Array);

                return byteArray;
            }
        }
    }
}
