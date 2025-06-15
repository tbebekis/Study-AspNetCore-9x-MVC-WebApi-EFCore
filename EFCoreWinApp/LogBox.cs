namespace EFCoreWinApp
{
    /// <summary>
    /// Represents a thread-safe log edit box.
    /// <para>To be used by Form projects in order to display log entries in a TextBox or RichTextBox.</para>
    /// </summary>
    static public class LogBox
    {
        const string SLine = "-------------------------------------------------------------------";
 
        static SynchronizationContext fSyncContext = AsyncOperationManager.SynchronizationContext;
        static TextBoxBase Box;

        /* private */
        static void DoClear(object Fake)
        {
            if (Box != null)
            {
                Box.Clear();
                Application.DoEvents();
            }
        }
        static void DoLog(string Text)
        {
            if (Box != null)
            {
                Box.AppendText(Text);
                if (Box.Text.Length > 0)
                {
                    Box.SelectionStart = Box.Text.Length;
                    Box.ScrollToCaret();
                }
                Application.DoEvents();
            }
        }

        /* public */
        /// <summary>
        /// Initializes this class.
        /// </summary>
        static public void Initialize(TextBoxBase Box)
        {
            if (LogBox.Box == null)
            {
                LogBox.Box = Box; 
            }

        }

        /// <summary>
        /// Clears the box
        /// </summary>
        static public void Clear()
        {
            fSyncContext.Post(o => DoClear(null), null);
        }
        /// <summary>
        /// Appends text in the box, in the last existing text line, if any.
        /// </summary>
        static public void Append(string Text)
        {
            if (!string.IsNullOrWhiteSpace(Text))
                fSyncContext.Post(o => DoLog(o as string), Text);
        }
        /// <summary>
        /// Appends a new text line in the box.
        /// </summary>
        static public void AppendLine(string Text)
        {
            if (string.IsNullOrWhiteSpace(Text))
                Text = Environment.NewLine;
            else if (Text == SLine)
                Text = Environment.NewLine + Text;
            else if (Text != SLine)
                Text = $"{Environment.NewLine}[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {Text} ";
            fSyncContext.Post(o => DoLog(o as string), Text);
        }
        /// <summary>
        /// Appends a new empty text line in the box.
        /// </summary>
        static public void AppendLineEmpty()
        {
            AppendLine(string.Empty);
        }
        /// <summary>
        /// Appends a new text line in the box.
        /// </summary>
        static public void AppendLine(Exception Ex)
        {
            AppendLine(Ex.ToString());
        }
        /// <summary>
        /// Appends a line in the box, i.e. a <c>-------------------------------------------------------------------</c>
        /// </summary>
        static public void AppendLine()
        {
            AppendLine(SLine);
        }

    }
}
