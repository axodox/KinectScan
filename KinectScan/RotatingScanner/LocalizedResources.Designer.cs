﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KinectScan {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class LocalizedResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal LocalizedResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KinectScan.LocalizedResources", typeof(LocalizedResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Calibration.
        /// </summary>
        internal static string CalibrationTitle {
            get {
                return ResourceManager.GetString("CalibrationTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Device not found! Please connect a Kinect device to the computer..
        /// </summary>
        internal static string DeviceNotFound {
            get {
                return ResourceManager.GetString("DeviceNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The connection with the device has been lost..
        /// </summary>
        internal static string DeviceRemoved {
            get {
                return ResourceManager.GetString("DeviceRemoved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error.
        /// </summary>
        internal static string ErrorCaption {
            get {
                return ResourceManager.GetString("ErrorCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Export went succesfully..
        /// </summary>
        internal static string ExportSuccesful {
            get {
                return ResourceManager.GetString("ExportSuccesful", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Export were unsuccesful..
        /// </summary>
        internal static string ExportUnsuccesful {
            get {
                return ResourceManager.GetString("ExportUnsuccesful", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Import went succesfully..
        /// </summary>
        internal static string ImportSuccesful {
            get {
                return ResourceManager.GetString("ImportSuccesful", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Import were unsuccesful..
        /// </summary>
        internal static string ImportUnsuccesful {
            get {
                return ResourceManager.GetString("ImportUnsuccesful", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Scanning finished..
        /// </summary>
        internal static string ScannerDone {
            get {
                return ResourceManager.GetString("ScannerDone", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Moving to start position....
        /// </summary>
        internal static string ScannerMovingToOrigin {
            get {
                return ResourceManager.GetString("ScannerMovingToOrigin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ready..
        /// </summary>
        internal static string ScannerReady {
            get {
                return ResourceManager.GetString("ScannerReady", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Scanning at {0} degrees..
        /// </summary>
        internal static string ScannerScanning {
            get {
                return ResourceManager.GetString("ScannerScanning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Settings.
        /// </summary>
        internal static string SettingsTitle {
            get {
                return ResourceManager.GetString("SettingsTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Adjust the mirror to the correct angle: you should barely see the side of the turntable in the correct position on the bottom of the image. Also check the image for interference and reflections, which may disrupt the scanner. Click next to continue..
        /// </summary>
        internal static string Step1 {
            get {
                return ResourceManager.GetString("Step1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Remove any objects from the turntable, then click next..
        /// </summary>
        internal static string Step2 {
            get {
                return ResourceManager.GetString("Step2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select the turntable on the image with the mouse: move the cursor to the upper-left corner of the imaginary boundary rectangle of the turntable, then hold down the left button, and move the mouse to the bottom-right corner and release the button. You may repeat this procedure to select the turntable correctly. Click next to continue..
        /// </summary>
        internal static string Step3 {
            get {
                return ResourceManager.GetString("Step3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Place the calibration etalon to the turntable: the etalon should face toward the scanner&apos;s mirror and it should snap in the holes on the turntable. Click next to continue..
        /// </summary>
        internal static string Step4 {
            get {
                return ResourceManager.GetString("Step4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select the etalon on the image with the mouse (like in step 2). Click next to preview the calibration results..
        /// </summary>
        internal static string Step5 {
            get {
                return ResourceManager.GetString("Step5", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Check if the the turntable&apos;s position has been selected correctly. Close the window to finish the calibration procedure..
        /// </summary>
        internal static string Step6 {
            get {
                return ResourceManager.GetString("Step6", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Turntable not found!.
        /// </summary>
        internal static string TurntableNotFound {
            get {
                return ResourceManager.GetString("TurntableNotFound", resourceCulture);
            }
        }
    }
}
