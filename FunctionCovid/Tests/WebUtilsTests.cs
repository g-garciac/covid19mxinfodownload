using System;
using Utils;
using Xunit;

namespace Tests
{
    public class WebUtilsTests
    {
        const string WebData = @"<html>
                                <body>
                                <a href=""http://remotehost.com/a/b/c/file1.png"">f1</a>
                                <div><a href=""http://remotehost.com/a/b/c/file2.zip"">f2</a></div>
                                <a href=""http://remotehost.com/a/b/c/file3.jpg"">f3</a>
                                <p><a href=""http://remotehost.com/a/b/c/file.pdf"">f</a></p>
                                <a href=""/x/y/z/file.dat"" target=""_blank"" onclick=""func('func', 'msg')""><span class=""cl1""></span><span class=""cl2"">f4</span></a>
                                </br>
                                <p><a href=""http://remotehost.com/a/file5.zip"">f5</a></p>
                                </body>
                                </html>";
        [Fact]
        public void FindUrisForFilesInWebPage_RelativeHost_ShouldbeOneUri()
        {
            var uris = WebUtils.FindUrisForFilesInWebPage(".dat", WebData, new Uri("https://myhost.com"));
            Assert.Single<Uri>(uris, u=>u.Equals(new Uri("https://myhost.com/x/y/z/file.dat")));
        }
        [Fact]
        public void FindUrisForFilesInWebPage_AbsoluteHost_ShouldbeTwoUris()
        {
            var uris = WebUtils.FindUrisForFilesInWebPage(".zip", WebData, new Uri("https://myhost.com"));
            Assert.Equal(2, uris.Count);
            Assert.Contains<Uri>(new Uri("http://remotehost.com/a/b/c/file2.zip"), uris);
            Assert.Contains<Uri>(new Uri("http://remotehost.com/a/file5.zip"), uris);
        }
    }
}
