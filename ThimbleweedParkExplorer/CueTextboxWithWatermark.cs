using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

class CueTextBox : TextBox
{
    [Localizable(true)]
    public string Cue
    {
        get { return mCue; }
        set { mCue = value; updateCue(); }
    }

    private void updateCue()
    {
        if (this.IsHandleCreated && mCue != null)
        {
            NativeMethods.SendMessage(this.Handle, 0x1501, (IntPtr)1, mCue);
        }
    }
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        updateCue();
    }
    private string mCue;

    internal static class NativeMethods
    {
        // PInvoke
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, string lp);
    }
}
