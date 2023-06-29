namespace MergeEditor
{
    public static class BuilderCfg
    {
        /// <summary>
        /// profile debug 模式
        /// </summary>
        public static int profile_debug = 0;
        /// <summary>
        /// 导出的AS工程目录
        /// </summary>
        public static string projectpath = "/Users/lyh/mergegame/CI/scripts/../../Build/IOS/MergeMiracle";
        /// <summary>
        /// keystore文件存放的地址
        /// </summary>
        public static string keyStoreFilePath = "/Users/lyh/mergegame/CI/scripts/../../CI/android/google/releasekeystore.keystore";
        public static string keystorePass = "yuanbo1234";
        public static string keyaliasName = "mergegame";
        public static string keyaliasPass = "yuanbo1234";
        
        public static string IosProvision = "miracle_adhoc";
        public static string IosProvisionUUID = "96d808e7-69d6-4420-9444-c3716cb303e6";
        public static string IosTeamID = "UW8FB5Y323";
        /// <summary>
        /// 用来做热更的版本
        /// </summary>
        public static string ResConfig = "";
        /// <summary>
        /// 设定小版本号
        /// </summary>
        public static string ResVersion = "0";
    }
}