using System.Diagnostics;
using System.Windows.Controls;
using Microsoft.Win32;

namespace RemoveDislike.Views.Pages;

public partial class AdjustmentPage
{
    public AdjustmentPage() => InitializeComponent();

    private static bool? RegValueEq(string key, string value, string target) => Registry.GetValue(
        key, value, null).ObjToStrEqRtBoolOrNull(target);
    
    private void Panel_OnInitialized(object sender, EventArgs e)
    {

        /*---------- Notifications ----------*/
        Put("Notifications",
            GRB("Disable",
                () => Registry.GetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\PushNotifications",
                    "ToastEnabled", null).ObjToStrEqRtBoolOrNull("00000000"),
                () =>
                {
                    Registry.SetValue(
                        "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\PushNotifications",
                        "ToastEnabled", "00000000", RegistryValueKind.DWord);

                    Process.Start(new ProcessStartInfo("powershell.exe")
                    {
                        Arguments = "Restart-Service -DisplayName 'Windows Push Notifications User Service*'",
                        CreateNoWindow = true
                    });
                }, true),
            GRB("Enable",
                () => Registry.GetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\PushNotifications",
                    "ToastEnabled", null).ObjToStrEqRtBoolOrNull("00000001"),
                () =>
                {
                    Registry.SetValue(
                        "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\PushNotifications",
                        "ToastEnabled", "00000001", RegistryValueKind.DWord);

                    Process.Start(new ProcessStartInfo("powershell.exe")
                    {
                        Arguments = "Restart-Service -DisplayName 'Windows Push Notifications User Service*'",
                        CreateNoWindow = true
                    });
                })
        );


        //Offer Suggestions
        Put("Offer Suggestions",
            GRB("Disable", () =>
                Registry.GetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement",
                    "ScoobeSystemSettingEnabled", null).ObjToStrEqRtBoolOrNull("00000000")
                , () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement",
                    "ScoobeSystemSettingEnabled", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () =>
                    RegValueEq("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement",
                                               "ScoobeSystemSettingEnabled", "00000001")
            , () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement",
                    "ScoobeSystemSettingEnabled", "00000001", RegistryValueKind.DWord);
            })
        );


        //Get Windows Tips
        Put("Get Windows Tips",
            GRB("Disable", 
                () => RegValueEq("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
                                                                "SubscribedContent-338389Enabled", "00000000"), () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
                    "SubscribedContent-338389Enabled", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () => RegValueEq("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
                                                               "SubscribedContent-338389Enabled", "00000001"), () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
                    "SubscribedContent-338389Enabled", "00000001", RegistryValueKind.DWord);
            })
        );

        /*---------- Storage ----------*/

        //Storage Sense
        Put("Storage Sense",
            GRB("Disable", () => RegValueEq( "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy",
                                                                "01", "00000000"), () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy",
                    "01", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () =>RegValueEq("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy",
                                                              "01", "00000001"), () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy",
                    "01", "00000001", RegistryValueKind.DWord);
            })
        );


        /*---------- Clipboard ----------*/
        //Clipboard History

        Put("Clipboard History",
            GRB("Disable", () => RegValueEq("HKEY_CURRENT_USER\\Software\\Microsoft\\Clipboard", "EnableClipboardHistory",
                                                                "00000000"), () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Clipboard", "EnableClipboardHistory",
                    "00000000",
                    RegistryValueKind.DWord);
            }),
            GRB("Enable", () => RegValueEq("HKEY_CURRENT_USER\\Software\\Microsoft\\Clipboard", "EnableClipboardHistory",
                                                               "00000001"), () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Clipboard", "EnableClipboardHistory",
                    "00000001",
                    RegistryValueKind.DWord);
            })
        );

        /*---------- Colors ----------*/
        // //Windows Theme
        Put("Windows Theme",
            GRB("Dark", () => RegValueEq("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                                                             "SystemUsesLightTheme", "00000000"), () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                    "SystemUsesLightTheme", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Light", () => RegValueEq("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                                                              "SystemUsesLightTheme", "00000001"), () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                    "SystemUsesLightTheme", "00000001", RegistryValueKind.DWord);
            })
        );

        // //Application Theme

        Put("Application Theme",
            GRB("Dark", () => RegValueEq("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                                                             "AppsUseLightTheme", "00000000"), () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                    "AppsUseLightTheme", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Light", () => RegValueEq("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                                                              "AppsUseLightTheme", "00000001"), () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                    "AppsUseLightTheme", "00000001", RegistryValueKind.DWord);
            })
        );

        // //Transparency Effects

        Put("Transparency Effects",
            GRB("Disable", () => RegValueEq("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                                                                "EnableTransparency", "00000000"), () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                    "EnableTransparency", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () =>RegValueEq("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                                                              "EnableTransparency", "00000001"), () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                    "EnableTransparency", "00000001", RegistryValueKind.DWord);
            })
        );

        /*---------- Start ----------*/
        //Show Recently Opened Items

        Put("Show Recently Opened Items",
            GRB("Disable", () => RegValueEq("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
                                                                "Start_TrackDocs", "00000000"), () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
                    "Start_TrackDocs", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () => RegValueEq("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
                                                               "Start_TrackDocs", "00000001"), () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
                    "Start_TrackDocs", "00000001", RegistryValueKind.DWord);
            })
        );

        /*---------- Gaming ----------*/

        Put("Game Mode",
            GRB("Disable", () => RegValueEq("HKEY_CURRENT_USER\\Software\\Microsoft\\GameBar", "AutoGameModeEnabled", "00000000"), () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\GameBar", "AutoGameModeEnabled", "00000000",
                    RegistryValueKind.DWord);
            }),
            GRB("Enable", () => RegValueEq("HKEY_CURRENT_USER\\Software\\Microsoft\\GameBar", "AutoGameModeEnabled", "00000001"), () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\GameBar", "AutoGameModeEnabled", "00000001",
                    RegistryValueKind.DWord);
            })
        );

        // /*---------- General ----------*/
        // //Let Apps Show Personalized Ads

        Put("Let Apps Show Personalized Ads",
            GRB("Disable", () => RegValueEq("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo",
                                                                "Enabled", "00000000").Value&&RegValueEq("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\CPSS\\Store\\AdvertisingInfo",
                                                                                                                       "Value", "00000000").Value, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo",
                    "Enabled", "00000000", RegistryValueKind.DWord);
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\CPSS\\Store\\AdvertisingInfo",
                    "Value", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo",
                    "Enabled", "00000001", RegistryValueKind.DWord);
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\CPSS\\Store\\AdvertisingInfo",
                    "Value", "00000001", RegistryValueKind.DWord);
            })
        );

        // //Allow Websites Access to Lnguage List
        // if (LanguageList_Disable.IsChecked == true)
        // {
        //     Registry.SetValue("HKEY_CURRENT_USER\\Control Panel\\International\\User Profile",
        //         "HttpAcceptLanguageOptOut", "00000001", RegistryValueKind.DWord);
        //
        //     var delreg = new ProcessStartInfo("WT.exe");
        //     delreg.UseShellExecute = true;
        //     delreg.Arguments =
        //         "REG DELETE \"HKCU\\Software\\Microsoft\\Internet Explorer\\International\" /v AcceptLanguage /f";
        //     Process.Start(delreg);
        // }

        Put("Allow Websites Access to Lnguage List",
            GRB("Disable", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Control Panel\\International\\User Profile",
                    "HttpAcceptLanguageOptOut", "00000001", RegistryValueKind.DWord);

                var delreg = new ProcessStartInfo("WT.exe");
                delreg.UseShellExecute = true;
                delreg.Arguments =
                    "REG DELETE \"HKCU\\Software\\Microsoft\\Internet Explorer\\International\" /v AcceptLanguage /f";
                Process.Start(delreg);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Control Panel\\International\\User Profile",
                    "HttpAcceptLanguageOptOut", "00000000", RegistryValueKind.DWord);

                var delreg = new ProcessStartInfo("WT.exe");
                delreg.UseShellExecute = true;
                delreg.Arguments =
                    "REG ADD \"HKCU\\Software\\Microsoft\\Internet Explorer\\International\" /v AcceptLanguage /t REG_SZ /d \"en-US,en;q=0.9\" /f";
                Process.Start(delreg);
            })
        );

        // //Improve Search by Tracking App Launches
        // if (ImproveSearch_Disable.IsChecked == true)
        // {
        //     Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
        //         "Start_TrackProgs", "00000000", RegistryValueKind.DWord);
        // }
        // else if (ImproveSearch_Enable.IsChecked == true)
        // {
        //     Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
        //         "Start_TrackProgs", "00000001", RegistryValueKind.DWord);
        // }

        Put("Improve Search by Tracking App Launches",
            GRB("Disable", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
                    "Start_TrackProgs", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
                    "Start_TrackProgs", "00000001", RegistryValueKind.DWord);
            })
        );

        // //Show Suggest Content in Settings
        // if (ShowSuggested_Disable.IsChecked == true)
        // {
        //     Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
        //         "SubscribedContent-353696Enabled", "00000000", RegistryValueKind.DWord);
        //     Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
        //         "SubscribedContent-353694Enabled", "00000000", RegistryValueKind.DWord);
        //     Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
        //         "SubscribedContent-338393Enabled", "00000000", RegistryValueKind.DWord);
        // }
        // else if (ShowSuggested_Enable.IsChecked == true)
        // {
        //     Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
        //         "SubscribedContent-353696Enabled", "00000001", RegistryValueKind.DWord);
        //     Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
        //         "SubscribedContent-353694Enabled", "00000001", RegistryValueKind.DWord);
        //     Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
        //         "SubscribedContent-338393Enabled", "00000001", RegistryValueKind.DWord);
        // }
        //

        Put("Show Suggest Content in Settings",
            GRB("Disable", () => { return false; }, () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
                    "SubscribedContent-353696Enabled", "00000000", RegistryValueKind.DWord);
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
                    "SubscribedContent-353694Enabled", "00000000", RegistryValueKind.DWord);
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
                    "SubscribedContent-338393Enabled", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
                    "SubscribedContent-353696Enabled", "00000001", RegistryValueKind.DWord);
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
                    "SubscribedContent-353694Enabled", "00000001", RegistryValueKind.DWord);
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager",
                    "SubscribedContent-338393Enabled", "00000001", RegistryValueKind.DWord);
            })
        );

        /*---------- Speech ----------*/
        //Online Speech Recognition
        //     if (SpeechRecognition_Disable.IsChecked == true)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Speech_OneCore\\Settings\\OnlineSpeechPrivacy",
        //             "HasAccepted", "00000000", RegistryValueKind.DWord);
        //     }
        //     else if (SpeechRecognition_Enable.IsChecked == true)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Speech_OneCore\\Settings\\OnlineSpeechPrivacy",
        //             "HasAccepted", "00000001", RegistryValueKind.DWord);
        //     }

        Put("Online Speech Recognition",
            GRB("Disable", () => { return false; }, () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Speech_OneCore\\Settings\\OnlineSpeechPrivacy",
                    "HasAccepted", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Speech_OneCore\\Settings\\OnlineSpeechPrivacy",
                    "HasAccepted", "00000001", RegistryValueKind.DWord);
            })
        );

        //     /*---------- Inking and Typing ----------*/
        //     //Personal Inking and Typing Dictionary
        //     if (Dictionary_Disable.IsChecked == true)
        //     {
        //         Registry.SetValue(
        //             "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\CPSS\\Store\\InkingAndTypingPersonalization",
        //             "Value", "00000000", RegistryValueKind.DWord);
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\InputPersonalization",
        //             "RestrictImplicitInkCollection", "00000001", RegistryValueKind.DWord);
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\InputPersonalization",
        //             "RestrictImplicitTextCollection", "00000001", RegistryValueKind.DWord);
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Personalization\\Settings",
        //             "AcceptedPrivacyPolicy", "00000000", RegistryValueKind.DWord);
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\InputPersonalization\\TrainedDataStore",
        //             "HarvestContacts", "0000000", RegistryValueKind.DWord);
        //     }
        //     else if (Dictionary_Enable.IsChecked == true)
        //     {
        //         Registry.SetValue(
        //             "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\CPSS\\Store\\InkingAndTypingPersonalization",
        //             "Value", "00000001", RegistryValueKind.DWord);
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\InputPersonalization",
        //             "RestrictImplicitInkCollection", "00000000", RegistryValueKind.DWord);
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\InputPersonalization",
        //             "RestrictImplicitTextCollection", "00000000", RegistryValueKind.DWord);
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Personalization\\Settings",
        //             "AcceptedPrivacyPolicy", "00000001", RegistryValueKind.DWord);
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\InputPersonalization\\TrainedDataStore",
        //             "HarvestContacts", "0000001", RegistryValueKind.DWord);
        //     }

        Put("Personal Inking and Typing Dictionary",
            GRB("Disable", () => { return false; }, () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\CPSS\\Store\\InkingAndTypingPersonalization",
                    "Value", "00000000", RegistryValueKind.DWord);
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\InputPersonalization",
                    "RestrictImplicitInkCollection", "00000001", RegistryValueKind.DWord);
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\InputPersonalization",
                    "RestrictImplicitTextCollection", "00000001", RegistryValueKind.DWord);
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Personalization\\Settings",
                    "AcceptedPrivacyPolicy", "00000000", RegistryValueKind.DWord);
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\InputPersonalization\\TrainedDataStore",
                    "HarvestContacts", "0000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\CPSS\\Store\\InkingAndTypingPersonalization",
                    "Value", "00000001", RegistryValueKind.DWord);
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\InputPersonalization",
                    "RestrictImplicitInkCollection", "00000000", RegistryValueKind.DWord);
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\InputPersonalization",
                    "RestrictImplicitTextCollection", "00000000", RegistryValueKind.DWord);
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Personalization\\Settings",
                    "AcceptedPrivacyPolicy", "00000001", RegistryValueKind.DWord);
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\InputPersonalization\\TrainedDataStore",
                    "HarvestContacts", "0000001", RegistryValueKind.DWord);
            }));

        //
        //     /*---------- Diagnostics and Feedback ----------*/
        //     //Send Optional Diagnostic Data
        //     if (OptionalData_Disable.IsChecked == true)
        //     {
        //         var P1 = new ProcessStartInfo("WT.exe");
        //         P1.UseShellExecute = true;
        //         P1.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection\" /v AllowTelemetry /t REG_DWORD /d \"00000001\" /f";
        //         Process.Start(P1);
        //         var P2 = new ProcessStartInfo("WT.exe");
        //         P2.UseShellExecute = true;
        //         P2.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection\" /v MaxTelemetryAllowed /t REG_DWORD /d \"00000001\" /f";
        //         Process.Start(P2);
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack",
        //             "ShowedToastAtLevel", "00000001", RegistryValueKind.DWord);
        //     }
        //     else if (OptionalData_Enable.IsChecked == true)
        //     {
        //         var P1 = new ProcessStartInfo("WT.exe");
        //         P1.UseShellExecute = true;
        //         P1.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection\" /v AllowTelemetry /t REG_DWORD /d \"00000003\" /f";
        //         Process.Start(P1);
        //         var P2 = new ProcessStartInfo("WT.exe");
        //         P2.UseShellExecute = true;
        //         P2.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection\" /v MaxTelemetryAllowed /t REG_DWORD /d \"00000003\" /f";
        //         Process.Start(P2);
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack",
        //             "ShowedToastAtLevel", "00000003", RegistryValueKind.DWord);
        //     }

        Put("Send Optional Diagnostic Data",
            GRB("Disable", () => { return false; }, () =>
            {
                var P1 = new ProcessStartInfo("WT.exe");
                P1.UseShellExecute = true;
                P1.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection\" /v AllowTelemetry /t REG_DWORD /d \"00000001\" /f";
                Process.Start(P1);
                var P2 = new ProcessStartInfo("WT.exe");
                P2.UseShellExecute = true;
                P2.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection\" /v MaxTelemetryAllowed /t REG_DWORD /d \"00000001\" /f";
                Process.Start(P2);
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack",
                    "ShowedToastAtLevel", "00000001", RegistryValueKind.DWord);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var P1 = new ProcessStartInfo("WT.exe");
                P1.UseShellExecute = true;
                P1.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection\" /v AllowTelemetry /t REG_DWORD /d \"00000003\" /f";
                Process.Start(P1);
                var P2 = new ProcessStartInfo("WT.exe");
                P2.UseShellExecute = true;
                P2.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection\" /v MaxTelemetryAllowed /t REG_DWORD /d \"00000003\" /f";
                Process.Start(P2);
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack",
                    "ShowedToastAtLevel", "00000003", RegistryValueKind.DWord);
            }));

        //     //Send Inking and Typing Data
        //     if (TypingData_Disable.IsChecked == true)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Input\\TIPC", "Enabled", "00000000",
        //             RegistryValueKind.DWord);
        //         Registry.SetValue(
        //             "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\CPSS\\Store\\ImproveInkingAndTyping",
        //             "Value", "00000000", RegistryValueKind.DWord);
        //     }
        //     else if (TypingData_Enable.IsChecked == true)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Input\\TIPC", "Enabled", "00000001",
        //             RegistryValueKind.DWord);
        //         Registry.SetValue(
        //             "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\CPSS\\Store\\ImproveInkingAndTyping",
        //             "Value", "00000001", RegistryValueKind.DWord);
        //     }

        Put("Send Inking and Typing Data",
            GRB("Disable", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Input\\TIPC", "Enabled", "00000000",
                    RegistryValueKind.DWord);
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\CPSS\\Store\\ImproveInkingAndTyping",
                    "Value", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Input\\TIPC", "Enabled", "00000001",
                    RegistryValueKind.DWord);
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\CPSS\\Store\\ImproveInkingAndTyping",
                    "Value", "00000001", RegistryValueKind.DWord);
            }));

        //     //Tailored Experience
        //     if (Tailored_Disable.IsChecked == true)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Privacy",
        //             "TailoredExperiencesWithDiagnosticDataEnabled", "00000000", RegistryValueKind.DWord);
        //     }
        //     else if (Tailored_Enable.IsChecked == true)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Privacy",
        //             "TailoredExperiencesWithDiagnosticDataEnabled", "00000001", RegistryValueKind.DWord);
        //     }

        Put("Tailored Experience",
            GRB("Disable", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Privacy",
                    "TailoredExperiencesWithDiagnosticDataEnabled", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Privacy",
                    "TailoredExperiencesWithDiagnosticDataEnabled", "00000001", RegistryValueKind.DWord);
            }));

        //     //Feedback Frequency
        //     if (Feedback_Auto.IsSelected)
        //     {
        //         var delreg = new ProcessStartInfo("WT.exe");
        //         delreg.UseShellExecute = true;
        //         delreg.Arguments = "REG DELETE \"HKCU\\Software\\Microsoft\\Siuf\\Rules\" /v NumberOfSIUFInPeriod  /f";
        //         Process.Start(delreg);
        //
        //         var delreg1 = new ProcessStartInfo("WT.exe");
        //         delreg1.UseShellExecute = true;
        //         delreg1.Arguments = "REG DELETE \"HKCU\\Software\\Microsoft\\Siuf\\Rules\" /v PeriodInNanoSeconds /f";
        //         Process.Start(delreg1);
        //     }
        //     else if (Feedback_Always.IsSelected)
        //     {
        //         var delreg = new ProcessStartInfo("WT.exe");
        //         delreg.UseShellExecute = true;
        //         delreg.Arguments = "REG DELETE \"HKCU\\Software\\Microsoft\\Siuf\\Rules\" /v NumberOfSIUFInPeriod  /f";
        //         Process.Start(delreg);
        //
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Siuf\\Rules", "PeriodInNanoSeconds", "100000000",
        //             RegistryValueKind.QWord);
        //     }
        //     else if (Feedback_Daily.IsSelected)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Siuf\\Rules", "NumberOfSIUFInPeriod", "1",
        //             RegistryValueKind.DWord);
        //
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Siuf\\Rules", "PeriodInNanoSeconds",
        //             "864000000000", RegistryValueKind.QWord);
        //     }
        //     else if (Feedback_Weekly.IsSelected)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Siuf\\Rules", "NumberOfSIUFInPeriod", "1",
        //             RegistryValueKind.DWord);
        //
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Siuf\\Rules", "PeriodInNanoSeconds",
        //             "6048000000000", RegistryValueKind.QWord);
        //     }
        //     else if (Feedback_Never.IsSelected)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Siuf\\Rules", "NumberOfSIUFInPeriod", "0",
        //             RegistryValueKind.DWord);
        //
        //         var delreg1 = new ProcessStartInfo("WT.exe");
        //         delreg1.UseShellExecute = true;
        //         delreg1.Arguments = "REG DELETE \"HKCU\\Software\\Microsoft\\Siuf\\Rules\" /v PeriodInNanoSeconds /f";
        //         Process.Start(delreg1);
        //     }

        Put("Feedback Frequency",
            GRB("Auto", () => { return false; }, () =>
            {
                var delreg = new ProcessStartInfo("WT.exe");
                delreg.UseShellExecute = true;
                delreg.Arguments = "REG DELETE \"HKCU\\Software\\Microsoft\\Siuf\\Rules\" /v NumberOfSIUFInPeriod  /f";
                Process.Start(delreg);

                var delreg1 = new ProcessStartInfo("WT.exe");
                delreg1.UseShellExecute = true;
                delreg1.Arguments = "REG DELETE \"HKCU\\Software\\Microsoft\\Siuf\\Rules\" /v PeriodInNanoSeconds /f";
                Process.Start(delreg1);
            }),
            GRB("Always", () => { return false; }, () =>
            {
                var delreg = new ProcessStartInfo("WT.exe");
                delreg.UseShellExecute = true;
                delreg.Arguments = "REG DELETE \"HKCU\\Software\\Microsoft\\Siuf\\Rules\" /v NumberOfSIUFInPeriod  /f";
                Process.Start(delreg);

                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Siuf\\Rules", "PeriodInNanoSeconds",
                    "100000000",
                    RegistryValueKind.QWord);
            }),
            GRB("Daily", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Siuf\\Rules", "NumberOfSIUFInPeriod", "1",
                    RegistryValueKind.DWord);

                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Siuf\\Rules", "PeriodInNanoSeconds",
                    "864000000000", RegistryValueKind.QWord);
            }),
            GRB("Weekly", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Siuf\\Rules", "NumberOfSIUFInPeriod", "1",
                    RegistryValueKind.DWord);

                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Siuf\\Rules", "PeriodInNanoSeconds",
                    "6048000000000", RegistryValueKind.QWord);
            }),
            GRB("Never", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Siuf\\Rules", "NumberOfSIUFInPeriod", "0",
                    RegistryValueKind.DWord);

                var delreg1 = new ProcessStartInfo("WT.exe");
                delreg1.UseShellExecute = true;
                delreg1.Arguments = "REG DELETE \"HKCU\\Software\\Microsoft\\Siuf\\Rules\" /v PeriodInNanoSeconds /f";
                Process.Start(delreg1);
            }));


        //     /*---------- Search Permissions ----------*/
        //     //Safe Search
        //     if (SafeSearch_Strict.IsChecked == true)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings",
        //             "SafeSearchMode", "00000002", RegistryValueKind.DWord);
        //     }
        //     else if (SafeSearch_Moderate.IsChecked == true)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings",
        //             "SafeSearchMode", "00000001", RegistryValueKind.DWord);
        //     }
        //     else if (SafeSearch_Off.IsChecked == true)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings",
        //             "SafeSearchMode", "00000000", RegistryValueKind.DWord);
        //     }

        Put("Search Permissions",
            GRB("Strict", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings",
                    "SafeSearchMode", "00000002", RegistryValueKind.DWord);
            }),
            GRB("Moderate", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings",
                    "SafeSearchMode", "00000001", RegistryValueKind.DWord);
            }),
            GRB("Off", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings",
                    "SafeSearchMode", "00000000", RegistryValueKind.DWord);
            }));

        //     //Store Search History
        //     if (SearchHistory_Disable.IsChecked == true)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings",
        //             "IsDeviceSearchHistoryEnabled", "00000000", RegistryValueKind.DWord);
        //     }
        //     else if (SearchHistory_Enable.IsChecked == true)
        //     {
        //         Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings",
        //             "IsDeviceSearchHistoryEnabled", "00000001", RegistryValueKind.DWord);
        //     }
        //

        Put("Store Search History",
            GRB("Disable", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings",
                    "IsDeviceSearchHistoryEnabled", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings",
                    "IsDeviceSearchHistoryEnabled", "00000001", RegistryValueKind.DWord);
            }));

        //     /*---------- App Permissions ----------*/
        //     //Location Services
        //     if (AP_Location_Disable.IsChecked == true)
        //     {
        //         var P1 = new ProcessStartInfo("WT.exe");
        //         P1.UseShellExecute = true;
        //         P1.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\location\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(P1);
        //         var P2 = new ProcessStartInfo("WT.exe");
        //         P2.UseShellExecute = true;
        //         P2.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows NT\\CurrentVersion\\Sensor\\Overrides\\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}\" /v SensorPermissionState /t REG_DWORD /d \"00000000\" /f";
        //         Process.Start(P2);
        //     }
        //     else if (AP_Location_Enable.IsChecked == true)
        //     {
        //         var P1 = new ProcessStartInfo("WT.exe");
        //         P1.UseShellExecute = true;
        //         P1.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\location\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(P1);
        //         var P2 = new ProcessStartInfo("WT.exe");
        //         P2.UseShellExecute = true;
        //         P2.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows NT\\CurrentVersion\\Sensor\\Overrides\\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}\" /v SensorPermissionState /t REG_DWORD /d \"00000001\" /f";
        //         Process.Start(P2);
        //     }

        Put("Location Services",
            GRB("Disable", () => { return false; }, () =>
            {
                var P1 = new ProcessStartInfo("WT.exe");
                P1.UseShellExecute = true;
                P1.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\location\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(P1);
                var P2 = new ProcessStartInfo("WT.exe");
                P2.UseShellExecute = true;
                P2.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows NT\\CurrentVersion\\Sensor\\Overrides\\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}\" /v SensorPermissionState /t REG_DWORD /d \"00000000\" /f";
                Process.Start(P2);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var P1 = new ProcessStartInfo("WT.exe");
                P1.UseShellExecute = true;
                P1.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\location\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(P1);
                var P2 = new ProcessStartInfo("WT.exe");
                P2.UseShellExecute = true;
                P2.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows NT\\CurrentVersion\\Sensor\\Overrides\\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}\" /v SensorPermissionState /t REG_DWORD /d \"00000001\" /f";
                Process.Start(P2);
            }));

        //     //Camera Access
        //     if (AP_Camera_Disable.IsChecked == true)
        //     {
        //         var camera = new ProcessStartInfo("WT.exe");
        //         camera.UseShellExecute = true;
        //         camera.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\webcam\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(camera);
        //     }
        //     else if (AP_Camera_Enable.IsChecked == true)
        //     {
        //         var camera = new ProcessStartInfo("WT.exe");
        //         camera.UseShellExecute = true;
        //         camera.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\webcam\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(camera);
        //     }

        Put("Camera Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var camera = new ProcessStartInfo("WT.exe");
                camera.UseShellExecute = true;
                camera.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\webcam\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(camera);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var camera = new ProcessStartInfo("WT.exe");
                camera.UseShellExecute = true;
                camera.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\webcam\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(camera);
            }));

        //     //Microphone Access
        //     if (AP_Microphone_Disable.IsChecked == true)
        //     {
        //         var mic = new ProcessStartInfo("WT.exe");
        //         mic.UseShellExecute = true;
        //         mic.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\microphone\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(mic);
        //     }
        //     else if (AP_Microphone_Enable.IsChecked == true)
        //     {
        //         var mic = new ProcessStartInfo("WT.exe");
        //         mic.UseShellExecute = true;
        //         mic.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\microphone\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(mic);
        //     }

        Put("Microphone Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var mic = new ProcessStartInfo("WT.exe");
                mic.UseShellExecute = true;
                mic.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\microphone\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(mic);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var mic = new ProcessStartInfo("WT.exe");
                mic.UseShellExecute = true;
                mic.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\microphone\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(mic);
            }));

        //     //Voice Activation
        //     if (AP_VActivation_Disable.IsChecked == true)
        //     {
        //         Registry.SetValue(
        //             "HKEY_CURRENT_USER\\Software\\Microsoft\\Speech_OneCore\\Settings\\VoiceActivation\\UserPreferenceForAllApps",
        //             "AgentActivationEnabled", "00000000", RegistryValueKind.DWord);
        //     }
        //     else if (AP_VActivation_Enable.IsChecked == true)
        //     {
        //         Registry.SetValue(
        //             "HKEY_CURRENT_USER\\Software\\Microsoft\\Speech_OneCore\\Settings\\VoiceActivation\\UserPreferenceForAllApps",
        //             "AgentActivationEnabled", "00000001", RegistryValueKind.DWord);
        //     }

        Put("Voice Activation",
            GRB("Disable", () => { return false; }, () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Speech_OneCore\\Settings\\VoiceActivation\\UserPreferenceForAllApps",
                    "AgentActivationEnabled", "00000000", RegistryValueKind.DWord);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                Registry.SetValue(
                    "HKEY_CURRENT_USER\\Software\\Microsoft\\Speech_OneCore\\Settings\\VoiceActivation\\UserPreferenceForAllApps",
                    "AgentActivationEnabled", "00000001", RegistryValueKind.DWord);
            }));

        //     //Notifications Access
        //     if (AP_Notifications_Disable.IsChecked == true)
        //     {
        //         var notif = new ProcessStartInfo("WT.exe");
        //         notif.UseShellExecute = true;
        //         notif.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\userNotificationListener\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(notif);
        //     }
        //     else if (AP_Notifications_Enable.IsChecked == true)
        //     {
        //         var notif = new ProcessStartInfo("WT.exe");
        //         notif.UseShellExecute = true;
        //         notif.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\userNotificationListener\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(notif);
        //     }

        Put("Notifications Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var notif = new ProcessStartInfo("WT.exe");
                notif.UseShellExecute = true;
                notif.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\userNotificationListener\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(notif);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var notif = new ProcessStartInfo("WT.exe");
                notif.UseShellExecute = true;
                notif.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\userNotificationListener\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(notif);
            }));

        //     //Account Information Access
        //     if (AP_AInfo_Disable.IsChecked == true)
        //     {
        //         var account = new ProcessStartInfo("WT.exe");
        //         account.UseShellExecute = true;
        //         account.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\userAccountInformation\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(account);
        //     }
        //     else if (AP_AInfo_Enable.IsChecked == true)
        //     {
        //         var account = new ProcessStartInfo("WT.exe");
        //         account.UseShellExecute = true;
        //         account.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\userAccountInformation\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(account);
        //     }

        Put("Account Information Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var account = new ProcessStartInfo("WT.exe");
                account.UseShellExecute = true;
                account.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\userAccountInformation\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(account);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var account = new ProcessStartInfo("WT.exe");
                account.UseShellExecute = true;
                account.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\userAccountInformation\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(account);
            }));

        //     //Contacts Access
        //     if (AP_Contacts_Disable.IsChecked == true)
        //     {
        //         var contacts = new ProcessStartInfo("WT.exe");
        //         contacts.UseShellExecute = true;
        //         contacts.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\contacts\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(contacts);
        //     }
        //     else if (AP_Contacts_Enable.IsChecked == true)
        //     {
        //         var contacts = new ProcessStartInfo("WT.exe");
        //         contacts.UseShellExecute = true;
        //         contacts.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\contacts\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(contacts);
        //     }

        Put("Contacts Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var contacts = new ProcessStartInfo("WT.exe");
                contacts.UseShellExecute = true;
                contacts.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\contacts\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(contacts);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var contacts = new ProcessStartInfo("WT.exe");
                contacts.UseShellExecute = true;
                contacts.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\contacts\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(contacts);
            }));

        //     // Calendar Access
        //     if (AP_Calendar_Disable.IsChecked == true)
        //     {
        //         var calendar = new ProcessStartInfo("WT.exe");
        //         calendar.UseShellExecute = true;
        //         calendar.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\appointments\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(calendar);
        //     }
        //     else if (AP_Calendar_Enable.IsChecked == true)
        //     {
        //         var calendar = new ProcessStartInfo("WT.exe");
        //         calendar.UseShellExecute = true;
        //         calendar.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\appointments\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(calendar);
        //     }

        Put("Calendar Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var calendar = new ProcessStartInfo("WT.exe");
                calendar.UseShellExecute = true;
                calendar.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\appointments\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(calendar);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var calendar = new ProcessStartInfo("WT.exe");
                calendar.UseShellExecute = true;
                calendar.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\appointments\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(calendar);
            }));

        //     //Phone Call Access
        //     if (AP_PCall_Disable.IsChecked == true)
        //     {
        //         var phonecall = new ProcessStartInfo("WT.exe");
        //         phonecall.UseShellExecute = true;
        //         phonecall.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\phoneCall\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(phonecall);
        //     }
        //     else if (AP_PCall_Enable.IsChecked == true)
        //     {
        //         var phonecall = new ProcessStartInfo("WT.exe");
        //         phonecall.UseShellExecute = true;
        //         phonecall.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\phoneCall\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(phonecall);
        //     }

        Put("Phone Call Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var phonecall = new ProcessStartInfo("WT.exe");
                phonecall.UseShellExecute = true;
                phonecall.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\phoneCall\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(phonecall);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var phonecall = new ProcessStartInfo("WT.exe");
                phonecall.UseShellExecute = true;
                phonecall.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\phoneCall\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(phonecall);
            }));

        //     //Call History Access
        //     if (AP_CHistory_Disable.IsChecked == true)
        //     {
        //         var phonehistory = new ProcessStartInfo("WT.exe");
        //         phonehistory.UseShellExecute = true;
        //         phonehistory.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\phoneCallHistory\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(phonehistory);
        //     }
        //     else if (AP_CHistory_Enable.IsChecked == true)
        //     {
        //         var phonehistory = new ProcessStartInfo("WT.exe");
        //         phonehistory.UseShellExecute = true;
        //         phonehistory.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\phoneCallHistory\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(phonehistory);
        //     }

        Put("Call History Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var phonehistory = new ProcessStartInfo("WT.exe");
                phonehistory.UseShellExecute = true;
                phonehistory.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\phoneCallHistory\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(phonehistory);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var phonehistory = new ProcessStartInfo("WT.exe");
                phonehistory.UseShellExecute = true;
                phonehistory.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\phoneCallHistory\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(phonehistory);
            }));

        //     //Email Access
        //     if (AP_Email_Disable.IsChecked == true)
        //     {
        //         var phonehistory = new ProcessStartInfo("WT.exe");
        //         phonehistory.UseShellExecute = true;
        //         phonehistory.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\email\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(phonehistory);
        //     }
        //     else if (AP_Email_Enable.IsChecked == true)
        //     {
        //         var phonehistory = new ProcessStartInfo("WT.exe");
        //         phonehistory.UseShellExecute = true;
        //         phonehistory.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\email\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(phonehistory);
        //     }

        Put("Email Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var phonehistory = new ProcessStartInfo("WT.exe");
                phonehistory.UseShellExecute = true;
                phonehistory.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\email\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(phonehistory);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var phonehistory = new ProcessStartInfo("WT.exe");
                phonehistory.UseShellExecute = true;
                phonehistory.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\email\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(phonehistory);
            }));

        //     //Tasks Access
        //     if (AP_Tasks_Disable.IsChecked == true)
        //     {
        //         var tasks = new ProcessStartInfo("WT.exe");
        //         tasks.UseShellExecute = true;
        //         tasks.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\userDataTasks\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(tasks);
        //     }
        //     else if (AP_Tasks_Enable.IsChecked == true)
        //     {
        //         var tasks = new ProcessStartInfo("WT.exe");
        //         tasks.UseShellExecute = true;
        //         tasks.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\userDataTasks\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(tasks);
        //     }

        Put("Tasks Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var tasks = new ProcessStartInfo("WT.exe");
                tasks.UseShellExecute = true;
                tasks.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\userDataTasks\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(tasks);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var tasks = new ProcessStartInfo("WT.exe");
                tasks.UseShellExecute = true;
                tasks.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\userDataTasks\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(tasks);
            }));

        //     //Messaging Access
        //     if (AP_Messaging_Disable.IsChecked == true)
        //     {
        //         var chat = new ProcessStartInfo("WT.exe");
        //         chat.UseShellExecute = true;
        //         chat.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\chat\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(chat);
        //     }
        //     else if (AP_Messaging_Enable.IsChecked == true)
        //     {
        //         var chat = new ProcessStartInfo("WT.exe");
        //         chat.UseShellExecute = true;
        //         chat.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\chat\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(chat);
        //     }

        Put("Messaging Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var chat = new ProcessStartInfo("WT.exe");
                chat.UseShellExecute = true;
                chat.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\chat\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(chat);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var chat = new ProcessStartInfo("WT.exe");
                chat.UseShellExecute = true;
                chat.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\chat\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(chat);
            }));

        //     //Radio Control Access
        //     if (AP_Radio_Disable.IsChecked == true)
        //     {
        //         var radios = new ProcessStartInfo("WT.exe");
        //         radios.UseShellExecute = true;
        //         radios.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\radios\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(radios);
        //     }
        //     else if (AP_Radio_Enable.IsChecked == true)
        //     {
        //         var radios = new ProcessStartInfo("WT.exe");
        //         radios.UseShellExecute = true;
        //         radios.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\radios\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(radios);
        //     }

        Put("Radio Control Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var radios = new ProcessStartInfo("WT.exe");
                radios.UseShellExecute = true;
                radios.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\radios\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(radios);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var radios = new ProcessStartInfo("WT.exe");
                radios.UseShellExecute = true;
                radios.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\radios\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(radios);
            }));

        //     //Communicate with Unpaired Devices
        //     if (AP_Communicate_Disable.IsChecked == true)
        //     {
        //         var bsync = new ProcessStartInfo("WT.exe");
        //         bsync.UseShellExecute = true;
        //         bsync.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\bluetoothSync\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(bsync);
        //     }
        //     else if (AP_Communicate_Enable.IsChecked == true)
        //     {
        //         var bsync = new ProcessStartInfo("WT.exe");
        //         bsync.UseShellExecute = true;
        //         bsync.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\bluetoothSync\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(bsync);
        //     }

        Put("Communicate with Unpaired Devices",
            GRB("Disable", () => { return false; }, () =>
            {
                var bsync = new ProcessStartInfo("WT.exe");
                bsync.UseShellExecute = true;
                bsync.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\bluetoothSync\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(bsync);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var bsync = new ProcessStartInfo("WT.exe");
                bsync.UseShellExecute = true;
                bsync.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\bluetoothSync\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(bsync);
            }));

        //     //App Diagnostics Access
        //     if (AP_Diagnostic_Disable.IsChecked == true)
        //     {
        //         var diag = new ProcessStartInfo("WT.exe");
        //         diag.UseShellExecute = true;
        //         diag.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\appDiagnostics\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(diag);
        //     }
        //     else if (AP_Diagnostic_Enable.IsChecked == true)
        //     {
        //         var diag = new ProcessStartInfo("WT.exe");
        //         diag.UseShellExecute = true;
        //         diag.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\appDiagnostics\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(diag);
        //     }

        Put("App Diagnostics Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var diag = new ProcessStartInfo("WT.exe");
                diag.UseShellExecute = true;
                diag.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\appDiagnostics\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(diag);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var diag = new ProcessStartInfo("WT.exe");
                diag.UseShellExecute = true;
                diag.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\appDiagnostics\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(diag);
            }));

        //     //Doucments Access
        //     if (AP_Documents_Disable.IsChecked == true)
        //     {
        //         var documents = new ProcessStartInfo("WT.exe");
        //         documents.UseShellExecute = true;
        //         documents.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\documentsLibrary\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(documents);
        //     }
        //     else if (AP_Documents_Enable.IsChecked == true)
        //     {
        //         var documents = new ProcessStartInfo("WT.exe");
        //         documents.UseShellExecute = true;
        //         documents.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\documentsLibrary\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(documents);
        //     }

        Put("Documents Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var documents = new ProcessStartInfo("WT.exe");
                documents.UseShellExecute = true;
                documents.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\documentsLibrary\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(documents);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var documents = new ProcessStartInfo("WT.exe");
                documents.UseShellExecute = true;
                documents.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\documentsLibrary\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(documents);
            }));

        //     //Downloads Folder Access
        //     if (AP_Downloads_Disable.IsChecked == true)
        //     {
        //         var downloads = new ProcessStartInfo("WT.exe");
        //         downloads.UseShellExecute = true;
        //         downloads.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\downloadsFolder\" /v Value /t REG_SZ /d \"Deny\" /f";
        //         Process.Start(downloads);
        //     }
        //     else if (AP_Downloads_Enable.IsChecked == true)
        //     {
        //         var downloads = new ProcessStartInfo("WT.exe");
        //         downloads.UseShellExecute = true;
        //         downloads.Arguments =
        //             "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\downloadsFolder\" /v Value /t REG_SZ /d \"Allow\" /f";
        //         Process.Start(downloads);
        //     }

        Put("Downloads Folder Access",
            GRB("Disable", () => { return false; }, () =>
            {
                var downloads = new ProcessStartInfo("WT.exe");
                downloads.UseShellExecute = true;
                downloads.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\downloadsFolder\" /v Value /t REG_SZ /d \"Deny\" /f";
                Process.Start(downloads);
            }),
            GRB("Enable", () => { return false; }, () =>
            {
                var downloads = new ProcessStartInfo("WT.exe");
                downloads.UseShellExecute = true;
                downloads.Arguments =
                    "REG ADD \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\downloadsFolder\" /v Value /t REG_SZ /d \"Allow\" /f";
                Process.Start(downloads);
            }));
    }

    private void Put(string title, params RadioButton[] checkBoxes)
    {
        StackPanel p = new() { Orientation = Orientation.Horizontal };
        Panel.Children.Add(new GroupBox { Header = title, Content = p });
        foreach (RadioButton c in checkBoxes)
        {
            c.GroupName = title;
            p.Children.Add(c);
        }
    }

    /// <summary>
    ///     see private static CheckBox GetCheckBox(string title, Delegate getValue, Delegate setValue)
    /// </summary>
    /// <returns></returns>
    private static RadioButton GRB(string title, BoolDelegate getValue, VoidDelegate setValue,
        bool isDefault = false) =>
        GetCheckBox(title, getValue, setValue, isDefault);

    private static RadioButton GetCheckBox(string title, BoolDelegate getValue, VoidDelegate setValue,
        bool isDefault = false)
    {
        RadioButton c = new()
        {
            Content = title,
            IsChecked = getValue.Invoke() ?? isDefault
        };
        c.Checked += (_, _) =>
        {
            setValue.Invoke();
            c.IsChecked = getValue.Invoke();
        };
        return c;
    }


    private delegate bool? BoolDelegate();

    private delegate void VoidDelegate();
}