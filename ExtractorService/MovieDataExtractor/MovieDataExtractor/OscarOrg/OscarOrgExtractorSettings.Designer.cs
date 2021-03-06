﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MovieDataExtractor.OscarOrg {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.7.0.0")]
    internal sealed partial class OscarOrgExtractorSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static OscarOrgExtractorSettings defaultInstance = ((OscarOrgExtractorSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new OscarOrgExtractorSettings())));
        
        public static OscarOrgExtractorSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://www.oscars.org/oscars/ceremonies/{0}")]
        public string BaseUrl {
            get {
                return ((string)(this["BaseUrl"]));
            }
            set {
                this["BaseUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oscarArg_Awards.csv")]
        public string FileNameAwards {
            get {
                return ((string)(this["FileNameAwards"]));
            }
            set {
                this["FileNameAwards"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oscarArg_Memorable.csv")]
        public string FileNameMemorableMoments {
            get {
                return ((string)(this["FileNameMemorableMoments"]));
            }
            set {
                this["FileNameMemorableMoments"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oscarArg_AwardsPics.csv")]
        public string FileNameAwardsPics {
            get {
                return ((string)(this["FileNameAwardsPics"]));
            }
            set {
                this["FileNameAwardsPics"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oscarArg_AwardsMetadata.csv")]
        public string FileNameMetadata {
            get {
                return ((string)(this["FileNameMetadata"]));
            }
            set {
                this["FileNameMetadata"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Year,Type,WinOrLose,Key,Item")]
        public string ColumnNameAwards {
            get {
                return ((string)(this["ColumnNameAwards"]));
            }
            set {
                this["ColumnNameAwards"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Year,Moments")]
        public string ColumNameMemorableMoments {
            get {
                return ((string)(this["ColumNameMemorableMoments"]));
            }
            set {
                this["ColumNameMemorableMoments"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Year,Type,Content,Title,Description,Youtube,Url")]
        public string ColumnNameAwardsPics {
            get {
                return ((string)(this["ColumnNameAwardsPics"]));
            }
            set {
                this["ColumnNameAwardsPics"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Year,Location,Date,Honoring")]
        public string ColumnNameMetadata {
            get {
                return ((string)(this["ColumnNameMetadata"]));
            }
            set {
                this["ColumnNameMetadata"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//*[@id=\'quicktabs-tabpage-honorees-0\']/div/div[2]/div[@class=\'view-grouping\']/di" +
            "v")]
        public string XpathAwardsCategoryYearSpecific {
            get {
                return ((string)(this["XpathAwardsCategoryYearSpecific"]));
            }
            set {
                this["XpathAwardsCategoryYearSpecific"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("./div[contains(@class, \'views-field-field-actor-name\')]")]
        public string XpathFieldKeyName {
            get {
                return ((string)(this["XpathFieldKeyName"]));
            }
            set {
                this["XpathFieldKeyName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("./div[contains(@class, \'views-field-title\')]")]
        public string XpathFieldKeyValue {
            get {
                return ((string)(this["XpathFieldKeyValue"]));
            }
            set {
                this["XpathFieldKeyValue"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//div[contains(@class,\'field--name-field-ceremonies-media\')]")]
        public string XpathOscarHighlightPics {
            get {
                return ((string)(this["XpathOscarHighlightPics"]));
            }
            set {
                this["XpathOscarHighlightPics"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("../div[contains(@class,\'field--name-field-caption\')]")]
        public string XpathPicsCaption {
            get {
                return ((string)(this["XpathPicsCaption"]));
            }
            set {
                this["XpathPicsCaption"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".//a")]
        public string XpathPicsLink {
            get {
                return ((string)(this["XpathPicsLink"]));
            }
            set {
                this["XpathPicsLink"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//div[contains(@class,\'views-field-field-location-name\')]")]
        public string XpathMetadataOscarTitle {
            get {
                return ((string)(this["XpathMetadataOscarTitle"]));
            }
            set {
                this["XpathMetadataOscarTitle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//div[contains(@class,\'views-field-field-date\')]")]
        public string XpathMetadataOscarDate {
            get {
                return ((string)(this["XpathMetadataOscarDate"]));
            }
            set {
                this["XpathMetadataOscarDate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//div[contains(@class,\'views-field-field-honoring-line\')]")]
        public string XpathMetadataOscarHonoringDate {
            get {
                return ((string)(this["XpathMetadataOscarHonoringDate"]));
            }
            set {
                this["XpathMetadataOscarHonoringDate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1929")]
        public int ProcessStartYearOfOscar {
            get {
                return ((int)(this["ProcessStartYearOfOscar"]));
            }
            set {
                this["ProcessStartYearOfOscar"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2018")]
        public int ProcessEndYearOfOscar {
            get {
                return ((int)(this["ProcessEndYearOfOscar"]));
            }
            set {
                this["ProcessEndYearOfOscar"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/memorable-moments")]
        public string MomentsRelativeUrl {
            get {
                return ((string)(this["MomentsRelativeUrl"]));
            }
            set {
                this["MomentsRelativeUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//div[contains(@class,\'field--name-field-hero-image\')]//img")]
        public string XpathMomentPictureImg {
            get {
                return ((string)(this["XpathMomentPictureImg"]));
            }
            set {
                this["XpathMomentPictureImg"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("//div[contains(@class,\'field--type-text-with-summary\')]//div[contains(@class,\'eve" +
            "n\')]/*")]
        public string XpathMomentDiv {
            get {
                return ((string)(this["XpathMomentDiv"]));
            }
            set {
                this["XpathMomentDiv"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("{0}/preceding-sibling::node()[not(self::text())]")]
        public string XpathWinningElements {
            get {
                return ((string)(this["XpathWinningElements"]));
            }
            set {
                this["XpathWinningElements"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("{0}/following-sibling::node()[not(self::text())]")]
        public string XpathNominationElements {
            get {
                return ((string)(this["XpathNominationElements"]));
            }
            set {
                this["XpathNominationElements"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("./h3[2]")]
        public string XpathNominationHeader {
            get {
                return ((string)(this["XpathNominationHeader"]));
            }
            set {
                this["XpathNominationHeader"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SkipProcessing {
            get {
                return ((bool)(this["SkipProcessing"]));
            }
            set {
                this["SkipProcessing"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oscarOrg_Awards_Normalize.csv")]
        public string FileNameAwardsNormalize {
            get {
                return ((string)(this["FileNameAwardsNormalize"]));
            }
            set {
                this["FileNameAwardsNormalize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oscarOrg_Movies.csv")]
        public string FileNameMovies {
            get {
                return ((string)(this["FileNameMovies"]));
            }
            set {
                this["FileNameMovies"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oscarOrg_Persons.csv")]
        public string FileNamesPerson {
            get {
                return ((string)(this["FileNamesPerson"]));
            }
            set {
                this["FileNamesPerson"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oscarOrg_AwardType.csv")]
        public string FileNameAwardType {
            get {
                return ((string)(this["FileNameAwardType"]));
            }
            set {
                this["FileNameAwardType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Year,AwardTypeId,IsWinner,MovieId,PersonId")]
        public string ColumnNameAwardsNormalize {
            get {
                return ((string)(this["ColumnNameAwardsNormalize"]));
            }
            set {
                this["ColumnNameAwardsNormalize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PersonId,Name")]
        public string ColumnNamePerson {
            get {
                return ((string)(this["ColumnNamePerson"]));
            }
            set {
                this["ColumnNamePerson"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MovieId,Name")]
        public string ColumnNameMovies {
            get {
                return ((string)(this["ColumnNameMovies"]));
            }
            set {
                this["ColumnNameMovies"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("TypeId,Name")]
        public string ColumnNameAwardType {
            get {
                return ((string)(this["ColumnNameAwardType"]));
            }
            set {
                this["ColumnNameAwardType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MovieId,AwardTypeId,Name")]
        public string ColumnNameMusicSong {
            get {
                return ((string)(this["ColumnNameMusicSong"]));
            }
            set {
                this["ColumnNameMusicSong"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oscarOrg_MusicSong.csv")]
        public string FileNameMusicSong {
            get {
                return ((string)(this["FileNameMusicSong"]));
            }
            set {
                this["FileNameMusicSong"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oscarOrg_Job.csv")]
        public string FileNameJob {
            get {
                return ((string)(this["FileNameJob"]));
            }
            set {
                this["FileNameJob"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("JobId,Name")]
        public string ColumnNameJob {
            get {
                return ((string)(this["ColumnNameJob"]));
            }
            set {
                this["ColumnNameJob"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("oscarOrg_PersonJob.csv")]
        public string FileNamePersonJob {
            get {
                return ((string)(this["FileNamePersonJob"]));
            }
            set {
                this["FileNamePersonJob"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PersonId,JobId")]
        public string ColumnNamePersonJob {
            get {
                return ((string)(this["ColumnNamePersonJob"]));
            }
            set {
                this["ColumnNamePersonJob"] = value;
            }
        }
    }
}
