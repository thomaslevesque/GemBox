using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GemBox.WinForms
{
    public static class WinFormsExtensions
    {
        #region DragMove extension

        private static readonly Dictionary<IntPtr, DraggedControl> DraggedControls = new Dictionary<IntPtr, DraggedControl>();

        /// <summary>
        /// Active temporairement le comportement "DragMove" pour le contrôle spécifié, jusqu'à ce que le bouton
        /// de la souris soit relaché
        /// </summary>
        /// <param name="ctl">Le contrôle à déplacer</param>
        public static void DragMove(this Control ctl)
        {
            Point absolutePosition = Control.MousePosition;
            Point relativePosition = ctl.PointToClient(absolutePosition);
            // ReSharper disable once ObjectCreationAsStatement
            new DraggedControl(ctl, relativePosition.X, relativePosition.Y);
        }

        /// <summary>
        /// Active ou désactive le comportement "DragMove" pour le contrôle spécifié
        /// </summary>
        /// <param name="ctl">Le contrôle pour lequel le DragMove doit être activé ou désactivé</param>
        /// <param name="value">Une valeur indiquant s'il faut activer (true) ou désactiver (false) le DragMove pour le contrôle</param>
        public static void EnableDragMove(this Control ctl, bool value)
        {
            if (value)
            {
                if (!DraggedControls.ContainsKey(ctl.Handle))
                    DraggedControls.Add(ctl.Handle, new DraggedControl(ctl));
            }
            else
            {
                if (DraggedControls.ContainsKey(ctl.Handle))
                {
                    DraggedControls[ctl.Handle].Dispose();
                    DraggedControls.Remove(ctl.Handle);
                }
            }
        }

        /// <summary>
        /// Teste si le DragMove est actif pour le contrôle spécifié
        /// </summary>
        /// <param name="ctl">Le contrôle à tester</param>
        /// <returns>true si le DragMove est actif pour ce contrôle; sinon, false</returns>
        public static bool IsDragMoveEnabled(this Control ctl)
        {
            return DraggedControls.ContainsKey(ctl.Handle);
        }

        private class DraggedControl : IDisposable
        {
            private readonly Control _target;
            private int _xStart;
            private int _yStart;
            private bool _isMoving;
            private readonly bool _isTemporary;

            public DraggedControl(Control target)
            {
                _target = target;
                _isMoving = false;
                _isTemporary = false;
                _target.MouseDown += Target_MouseDown;
                _target.MouseMove += Target_MouseMove;
                _target.MouseUp += Target_MouseUp;
                _target.Disposed += Target_Disposed;
            }

            public DraggedControl(Control target, int xStart, int yStart)
                : this(target)
            {
                _xStart = xStart;
                _yStart = yStart;
                _isMoving = (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left;
                _isTemporary = true;
            }

            void Target_MouseDown(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _isMoving = true;
                    _xStart = e.X;
                    _yStart = e.Y;
                }
            }

            void Target_MouseUp(object sender, MouseEventArgs e)
            {
                _isMoving = false;
                if (_isTemporary)
                {
                    Dispose();
                }
            }

            private void Target_MouseMove(object sender, MouseEventArgs e)
            {
                if (_isMoving)
                {
                    int x = _target.Location.X + e.X - _xStart;
                    int y = _target.Location.Y + e.Y - _yStart;
                    _target.Location = new Point(x, y);
                }
            }

            void Target_Disposed(object sender, EventArgs e)
            {
                _target.EnableDragMove(false);
            }

            public void Dispose()
            {
                _target.MouseDown -= Target_MouseDown;
                _target.MouseMove -= Target_MouseMove;
                _target.MouseUp -= Target_MouseUp;
                _target.Disposed -= Target_Disposed;
            }
        }

        #endregion

        #region Symbole "Bouclier" UAC pour les actions administratives

        #region UAC Shield Interop

        [DllImport("user32")]
        static extern UInt32 SendMessage
            (IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);

        const int BcmFirst = 0x1600; //Normal button
        const int BcmSetshield = (BcmFirst + 0x000C); //Elevated button

        #endregion

        /// <summary>
        /// Rajoute le bouclier UAC si le processus actuel n'est pas
        /// élevé et qu'on est bien sur Windows Vista
        /// </summary>
        /// <param name="b">Le bouton pour lequel ajouter le bouclier UAC</param>
        public static void AddUacShieldIfNecessary(this Button b)
        {
            if (SystemInfo.SupportsElevation)
            {
                if (!SystemInfo.IsElevated)
                    b.AddUacShield();
            }
        }

        /// <summary>
        /// Affiche le bouclier UAC sur un bouton pour indiquer une action
        /// qui requiert les droits d'administrateur
        /// </summary>
        /// <param name="b">Le bouton pour lequel ajouter le bouclier UAC</param>
        public static void AddUacShield(this Button b)
        {
            if (SystemInfo.SupportsElevation)
            {
                SendMessage(b.Handle, BcmSetshield, 0, 0xFFFFFFFF);
            }
        }

        /// <summary>
        /// Enlève le bouclier UAC d'un bouton
        /// </summary>
        /// <param name="b">Le bouton pour lequel enlever le bouclier UAC</param>
        public static void RemoveUacShield(this Button b)
        {
            if (SystemInfo.SupportsElevation)
            {
                SendMessage(b.Handle, BcmSetshield, 0u, 0x00000000);
            }
        }

        #endregion

        #region Capture de l'image d'un contrôle

        #region Interop

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr hdc, PrfFlags drawingOptions);

        const uint WmPrint = 0x317;

        // ReSharper disable UnusedMember.Local
        [Flags]
        enum PrfFlags : uint
        {
            CheckVisible = 0x01,
            Children = 0x02,
            Client = 0x04,
            EraseBkgnd = 0x08,
            NonClient = 0x10,
            Owned = 0x20
        }
        // ReSharper restore UnusedMember.Local

        #endregion

        /// <summary>
        /// Capture le rendu d'un contrôle dans une image
        /// </summary>
        /// <param name="control">Le contrôle à capturer</param>
        /// <returns>L'image du contrôle</returns>
        public static Image CaptureImage(this Control control)
        {
            Image img = new Bitmap(control.Width, control.Height);
            using (Graphics g = Graphics.FromImage(img))
            {
                SendMessage(
                   control.Handle,
                   WmPrint,
                   g.GetHdc(),
                   PrfFlags.Client | PrfFlags.NonClient | PrfFlags.EraseBkgnd);
            }
            return img;
        }

        #endregion

        #region Localisation à la volée

        ///<summary>
        /// Applique aux composants d'une fenêtre ou d'un UserControl les ressources
        /// localisées pour la culture d'interface graphique courante (CurrentUICulture)
        ///</summary>
        ///<param name="control">La fenêtre ou le UserControl auquel appliquer les ressources localisées</param>
        ///<typeparam name="T">Type de la fenêtre ou du UserControl</typeparam>
        /// <remarks>Cette méthode permet permet le changement de culture "à la volée", sans recréer la form ou le UserControl.</remarks>
        public static void ApplyResources<T>(this T control)
            where T : Control
        {
            var resources = new ComponentResourceManager(typeof(T));
            var rs = resources.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            if (rs == null)
                return;

            var controlNames = rs.Cast<DictionaryEntry>()
                                 .Select(e => ExtractControlName((string)e.Key))
                                 .Distinct()
                                 .ToList();

            foreach (var name in controlNames)
            {
                object ctl = name == "$this"
                                 ? control
                                 : GetComponent(control, name);

                if (ctl != null)
                    resources.ApplyResources(ctl, name);
            }
        }

        private static object GetComponent<T>(T control, string name)
            where T : Control
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var field = typeof(T).GetField(name, flags);
            if (field != null && typeof(Component).IsAssignableFrom(field.FieldType))
                return field.GetValue(control);

            // In VB.NET, WithEvents fields are wrapped in properties
            var property = typeof(T).GetProperty(name, flags);
            if (property != null && typeof(Component).IsAssignableFrom(property.PropertyType))
                return property.GetValue(control, null);

            return null;
        }

        private static string ExtractControlName(string key)
        {
            return key.TrimStart('>').Split('.')[0];
        }

        #endregion

        #region InvokeIfRequired

        /// <summary>
        /// Exécute une action sur le thread de l'interface graphique, en utilisant Invoke si nécessaire.
        /// </summary>
        /// <param name="target">Contrôle sur le thread duquel l'action doit être exécutée</param>
        /// <param name="action">Action à exécuter</param>
        public static void InvokeIfRequired(this ISynchronizeInvoke target, Action action)
        {
            if (target.InvokeRequired)
                target.Invoke(action, null);
            else
                action();
        }

        /// <summary>
        /// Exécute une action sur le thread de l'interface graphique, en utilisant Invoke si nécessaire.
        /// </summary>
        /// <typeparam name="T">Type du contrôle sur lequel l'action est exécutée</typeparam>
        /// <param name="target">Contrôle sur le thread duquel l'action doit être exécutée</param>
        /// <param name="action">Action à exécuter</param>
        public static void InvokeIfRequired<T>(this T target, Action<T> action)
            where T : ISynchronizeInvoke
        {
            if (target.InvokeRequired)
                target.Invoke(action, new object[] { target });
            else
                action(target);
        }

        #endregion

        #region FlashWindow

        ///<summary>
        /// Fait clignoter une fenêtre pour attirer l'attention de l'utilisateur. Le clignotement s'arrête quand la fenêtre passe au premier plan.
        ///</summary>
        ///<param name="window">Fenêtre à faire clignoter</param>
        public static void FlashWindow(this IWin32Window window)
        {
            FlashWindow(window, FlashWindowFlags.All | FlashWindowFlags.UntilForeground);
        }

        ///<summary>
        /// Fait clignoter une fenêtre pour attirer l'attention de l'utilisateur.
        ///</summary>
        ///<param name="window">Fenêtre à faire clignoter</param>
        ///<param name="flags">Options de clignotement</param>
        public static void FlashWindow(this IWin32Window window, FlashWindowFlags flags)
        {
            int count = 3;
            if (flags.HasFlag(FlashWindowFlags.Continuous) || flags.HasFlag(FlashWindowFlags.UntilForeground))
                count = 0;
            FlashWindow(window, flags, count);
        }

        ///<summary>
        /// Fait clignoter une fenêtre pour attirer l'attention de l'utilisateur. Le clignotement s'arrête quand la fenêtre passe au premier plan.
        ///</summary>
        ///<param name="window">Fenêtre à faire clignoter</param>
        ///<param name="count">Nombre de clignotements</param>
        public static void FlashWindow(this IWin32Window window, int count)
        {
            FlashWindow(window, FlashWindowFlags.All | FlashWindowFlags.UntilForeground, count);
        }

        ///<summary>
        /// Fait clignoter une fenêtre pour attirer l'attention de l'utilisateur
        ///</summary>
        ///<param name="window">Fenêtre à faire clignoter</param>
        ///<param name="flags">Options de clignotement</param>
        ///<param name="count">Nombre de clignotements</param>
        public static void FlashWindow(this IWin32Window window, FlashWindowFlags flags, int count)
        {
            FlashWindow(window, flags, count, 0);
        }

        ///<summary>
        /// Fait clignoter une fenêtre pour attirer l'attention de l'utilisateur
        ///</summary>
        ///<param name="window">Fenêtre à faire clignoter</param>
        ///<param name="flags">Options de clignotement</param>
        ///<param name="count">Nombre de clignotements</param>
        ///<param name="intervalMilliseconds">Intervalle de temps, en millisecondes, entre 2 clignotements (0 : intervalle par défaut)</param>
        public static void FlashWindow(this IWin32Window window, FlashWindowFlags flags, int count, int intervalMilliseconds)
        {
            var fwi = new FLASHWINFO
            {
                cbSize = (uint) Marshal.SizeOf(typeof (FLASHWINFO)),
                hwnd = window.Handle,
                dwFlags = flags,
                uCount = (uint) count,
                dwTimeout = (uint) intervalMilliseconds
            };
            FlashWindowEx(ref fwi);
        }

        [DllImport("user32.dll")]
        private static extern bool FlashWindowEx(ref FLASHWINFO fwi);

        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable once InconsistentNaming
        private struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public FlashWindowFlags dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }

        #endregion
    }

    [Flags]
    public enum FlashWindowFlags : uint
    {
        Stop = 0x00,
        Caption = 0x01,
        Taskbar = 0x02,
        All = 0x03,
        Continuous = 0x04,
        UntilForeground = 0x0C
    }
}
