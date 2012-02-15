using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Devel_VM
{
    [Guid("ed109b6e-0578-4b17-8ace-52646789f1a0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport()]
    interface ICustomGuest
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        String get_OSTypeId();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        UInt32 get_AdditionsRunLevel();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        String get_AdditionsVersion();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        IntPtr get_Facilities();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        UInt32 get_MemoryBalloonSize();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void set_MemoryBalloonSize(UInt32 value);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        UInt32 get_StatisticsUpdateInterval();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void set_StatisticsUpdateInterval(UInt32 value);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void InternalGetStatistics(out UInt32 pcpuUserout, UInt32 pcpuKernelout, UInt32 pcpuIdleout, UInt32 pmemTotalout, UInt32 pmemFreeout, UInt32 pmemBalloonout, UInt32 pmemSharedout, UInt32 pmemCacheout, UInt32 ppagedTotalout, UInt32 pmemAllocTotalout, UInt32 pmemFreeTotalout, UInt32 pmemBalloonTotalout, UInt32 pmemSharedTotal);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        UInt32 GetFacilityStatus(UInt32 pfacilityout, Int64 ptimestamp);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Bool)]
        Boolean GetAdditionsStatus(UInt32 plevel);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetCredentials([MarshalAs(UnmanagedType.LPWStr)] String puserName, [MarshalAs(UnmanagedType.LPWStr)] String ppassword, [MarshalAs(UnmanagedType.LPWStr)] String pdomain, [MarshalAs(UnmanagedType.Bool)] Boolean pallowInteractiveLogon);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        IntPtr ExecuteProcess([MarshalAs(UnmanagedType.LPWStr)] String pexecName, UInt32 pflags, System.Array parguments, System.Array penvironment, [MarshalAs(UnmanagedType.LPWStr)] String puserName, [MarshalAs(UnmanagedType.LPWStr)] String ppassword, UInt32 ptimeoutMSout, UInt32 ppid);

        /*[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]*/

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Byte[] GetProcessOutput(UInt32 ppid, UInt32 pflags, UInt32 ptimeoutMS, Int64 psize);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        UInt32 GetProcessStatus(UInt32 ppidout, UInt32 pexitcodeout, UInt32 pflags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        IntPtr CopyToGuest([MarshalAs(UnmanagedType.LPWStr)] String psource, [MarshalAs(UnmanagedType.LPWStr)] String pdest, [MarshalAs(UnmanagedType.LPWStr)] String puserName, [MarshalAs(UnmanagedType.LPWStr)] String ppassword, UInt32 pflags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        IntPtr CreateDirectory([MarshalAs(UnmanagedType.LPWStr)] String pdirectory, [MarshalAs(UnmanagedType.LPWStr)] String puserName, [MarshalAs(UnmanagedType.LPWStr)] String ppassword, UInt32 pmode, UInt32 pflags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        UInt32 SetProcessInput(UInt32 ppid, UInt32 pflags, UInt32 ptimeoutMS, System.Array pdata);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        IntPtr UpdateGuestAdditions([MarshalAs(UnmanagedType.LPWStr)] String psource, UInt32 pflags);
    }
}
