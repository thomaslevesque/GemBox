using System;
using System.Runtime.InteropServices;

namespace GemBox.WinForms
{
    class SystemInfo
    {
        public static bool SupportsElevation
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT
                    && Environment.OSVersion.Version.Major >= 6;
            }
        }

        public static bool IsElevated
        {
            get { return SupportsElevation && IsElevatedCore(); }
        }

        private static bool IsElevatedCore()
        {
            IntPtr hToken;
            int sizeofTokenElevationType = Marshal.SizeOf(typeof(int));
            IntPtr pElevationType =
                Marshal.AllocHGlobal(sizeofTokenElevationType);

            if (OpenProcessToken(GetCurrentProcess(), TokenQuery, out hToken))
            {
                uint dwSize;
                if (GetTokenInformation(hToken,
                    TokenInformationClass.TokenElevationType, pElevationType,
                    (uint)sizeofTokenElevationType, out dwSize))
                {
                    TokenElevationType elevationType = (TokenElevationType)Marshal.ReadInt32(pElevationType);
                    Marshal.FreeHGlobal(pElevationType);

                    switch (elevationType)
                    {
                        case TokenElevationType.TokenElevationTypeFull:
                            return true;
                        default:
                            //case TokenElevationType.TokenElevationTypeLimited:
                            //case TokenElevationType.TokenElevationTypeDefault:
                            return false;
                    }
                }
            }

            return false;
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool OpenProcessToken(
            IntPtr processHandle,
            uint desiredAccess,
            out IntPtr tokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetTokenInformation(
            IntPtr tokenHandle,
            TokenInformationClass tokenInformationClass,
            IntPtr tokenInformation,
            uint tokenInformationLength,
            out uint returnLength);

        // ReSharper disable UnusedMember.Local

        enum TokenElevationType
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }

        enum TokenInformationClass
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }

        // ReSharper restore UnusedMember.Local
        const UInt32 TokenQuery = 0x0008;


    }
}
