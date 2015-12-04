#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.IO;
using System.Net;

#if PORTABLE
using System.Threading.Tasks;
#endif

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal class WebRequestDownloader : IDownloader
    {
        public Stream GetStream(Uri uri, ICredentials credentials, int? timeout, int? byteLimit)
        {
            WebRequest request = WebRequest.Create(uri);
#if !PORTABLE
            if (timeout != null)
            {
                request.Timeout = timeout.Value;
            }
#endif

            if (credentials != null)
            {
                request.Credentials = credentials;
            }

            WebResponse response;

#if PORTABLE
            Task<WebResponse> result = Task.Factory.FromAsync(
                request.BeginGetResponse,
                new Func<IAsyncResult, WebResponse>(request.EndGetResponse), null);
            result.ConfigureAwait(false);

            response = result.Result;
#else
            response = request.GetResponse();
#endif

            Stream responseStream = response.GetResponseStream();
            if (timeout != null)
            {
                responseStream.ReadTimeout = timeout.Value;
            }

            if (byteLimit != null)
            {
                return new LimitedStream(responseStream, byteLimit.Value);
            }

            return responseStream;
        }
    }

    internal class LimitedStream : Stream
    {
        private readonly Stream _stream;
        private readonly int _byteLimit;
        private int _totalBytesRead;

        public LimitedStream(Stream stream, int byteLimit)
        {
            _stream = stream;
            _byteLimit = byteLimit;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = _stream.Read(buffer, offset, count);
            checked
            {
                _totalBytesRead += bytesRead;
            }

            if (_totalBytesRead > _byteLimit)
            {
                throw new IOException("Maximum read limit exceeded");
            }

            return bytesRead;
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }
    }
}