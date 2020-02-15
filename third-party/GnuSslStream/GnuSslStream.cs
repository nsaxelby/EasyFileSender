// This file isn't generated, but this comment is necessary to exclude it from StyleCop analysis.
// <auto-generated/>

using System.IO;

/* About the annoying BUG in SslStream:
 * .NET's SslStream class does not send the close_notify alert before closing the connection,
 * which is needed by GnuTLS. If we don't send this signal, GnuTLS will throw an error like this:
 * [GnuTLS error -110: The TLS connection was non-properly terminated.]
 * So we have to fix this issue by ourselves.(Microsoft refused to fix this, see:https://connect.microsoft.com/VisualStudio/feedback/details/788752/sslstream-does-not-properly-send-the-close-notify-alert)
 * The Following solution was provided by Neco(Nikolay Uvaliyev) at http://stackoverflow.com/questions/237807/net-sslstream-doesnt-close-tls-connection-properly .
 * Much Thanks to him!
 * P.S. SslStream in Mono works correctly.
 * by Ulysses , 2014
 */
/* 关于SslStream的BUG：
 * .NET的SslStream在关闭之前不会发送close_notify信号，而在很多TLS库（除了微软）之外是需要这个信号的。
 * 如果不发这个信号，GnuTLS就会报错：服务器没有正常的关闭 TLS 连接。微软表示现阶段不会修复这个问题（网址见上）。
 * 这个解决方法是StackOverflow的Neco(Nikolay Uvaliyev)提供的（这位老外的昵称难道是Neko（猫）吗……网址见上）。非常感谢他！
 * 经验证，Mono中的Sslstream无此问题！
 * by Ulysses , 2014
 */


namespace System.Net.Security
{
    /// <summary>
    /// An SslStream implementation that gracefully terminates the SSL stream.
    /// </summary>
    /// <remarks>
    /// This implementation works only(!) on Windows and is used to fix a misbehaviour
    /// in combination with applications using GnuTLS.
    /// </remarks>
    public class GnuSslStream : SslStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GnuSslStream"/> class.
        /// </summary>
        /// <param name="innerStream">The inner (network) stream.</param>
        public GnuSslStream(Stream innerStream)
            : base(innerStream)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GnuSslStream"/> class.
        /// </summary>
        /// <param name="innerStream">The inner (network) stream.</param>
        /// <param name="leaveInnerStreamOpen">Leave the inner stream open.</param>
        public GnuSslStream(Stream innerStream, bool leaveInnerStreamOpen)
            : base(innerStream, leaveInnerStreamOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GnuSslStream"/> class.
        /// </summary>
        /// <param name="innerStream">The inner (network) stream.</param>
        /// <param name="leaveInnerStreamOpen">Leave the inner stream open.</param>
        /// <param name="userCertificateValidationCallback">A callback allowing validation of user certificates.</param>
        public GnuSslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback)
            : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GnuSslStream"/> class.
        /// </summary>
        /// <param name="innerStream">The inner (network) stream.</param>
        /// <param name="leaveInnerStreamOpen">Leave the inner stream open.</param>
        /// <param name="userCertificateValidationCallback">A callback allowing validation of user certificates.</param>
        /// <param name="userCertificateSelectionCallback">A callback for the user certificate selection.</param>
        public GnuSslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback)
            : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback)
        {
        }
        //Mono不支持此重载构造函数 索性取消
#if WINDOWS
        public GnuSslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback, EncryptionPolicy encryptionPolicy)
            : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback, encryptionPolicy)
        {
        }
#endif

        /// <inheritdoc />
        public override void Close()
        {
            //MARK: SslStream in Mono works correctly.
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                base.Close();
                return;
            }
            try
            {
                SslDirectCall.CloseNotify(this);
                base.Flush();
            }
            finally
            {
                base.Close();
            }
        }
    }
}
