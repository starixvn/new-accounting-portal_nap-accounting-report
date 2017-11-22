namespace Fanex.ServiceManagement.Utility
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Runtime.InteropServices;

    public enum ResourceScope : int
    {
        /// <summary>
        /// The connected.
        /// </summary>
        Connected = 1,

        /// <summary>
        /// The global network.
        /// </summary>
        GlobalNetwork,

        /// <summary>
        /// The remembered.
        /// </summary>
        Remembered,

        /// <summary>
        /// The recent.
        /// </summary>
        Recent,

        /// <summary>
        /// The context.
        /// </summary>
        Context
    }

    public enum ResourceType : int
    {
        /// <summary>
        /// Resource type Any.
        /// </summary>
        Any = 0,

        /// <summary>
        /// The disk type.
        /// </summary>
        Disk = 1,

        /// <summary>
        /// The print type.
        /// </summary>
        Print = 2,

        /// <summary>
        /// The reserved.
        /// </summary>
        Reserved = 8,
    }

    public enum ResourceDisplaytype : int
    {
        /// <summary>
        /// The generic.
        /// </summary>
        Generic = 0x0,

        /// <summary>
        /// The domain.
        /// </summary>
        Domain = 0x01,

        /// <summary>
        /// The server.
        /// </summary>
        Server = 0x02,

        /// <summary>
        /// The share.
        /// </summary>
        Share = 0x03,

        /// <summary>
        /// The file.
        /// </summary>
        File = 0x04,

        /// <summary>
        /// The group.
        /// </summary>
        Group = 0x05,

        /// <summary>
        /// The network.
        /// </summary>
        Network = 0x06,

        /// <summary>
        /// The root.
        /// </summary>
        Root = 0x07,

        /// <summary>
        /// The share admin.
        /// </summary>
        Shareadmin = 0x08,

        /// <summary>
        /// The directory.
        /// </summary>
        Directory = 0x09,

        /// <summary>
        /// The tree.
        /// </summary>
        Tree = 0x0a,

        /// <summary>
        /// The NDS container.
        /// </summary>
        Ndscontainer = 0x0b
    }

    /// <summary>
    /// Net Resource.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("T4", "1.0.0.0")]
    [StructLayout(LayoutKind.Sequential)]
    public class NetResource
    {
        /// <summary>
        /// Resource scope.
        /// </summary>
        public ResourceScope Scope;

        /// <summary>
        /// Resource type.
        /// </summary>
        public ResourceType ResourceType;

        /// <summary>
        /// Display type.
        /// </summary>
        public ResourceDisplaytype DisplayType;

        /// <summary>
        /// The usage.
        /// </summary>
        public int Usage;

        /// <summary>
        /// The local name.
        /// </summary>
        public string LocalName;

        /// <summary>
        /// The remote name.
        /// </summary>
        public string RemoteName;

        /// <summary>
        /// The comment.
        /// </summary>
        public string Comment;

        /// <summary>
        /// The provider.
        /// </summary>
        public string Provider;
    }

    /// <summary>
    /// Network Connection.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("T4", "1.0.0.0")]
    public class NetworkConnection : IDisposable
    {
        string _networkName;
        public NetworkConnection(string networkName, NetworkCredential credentials)
        {
            _networkName = networkName;

            var netResource = new NetResource()
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            };

            WNetCancelConnection2(_networkName, 0, true);
            var result = WNetAddConnection2(
                netResource,
                credentials.Password,
                credentials.UserName,
                0);

            if (result != 0)
            {
                throw new Win32Exception(result, "Error connecting to remote share");
            }
        }

        ~NetworkConnection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_networkName, 0, true);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);
    }
}
