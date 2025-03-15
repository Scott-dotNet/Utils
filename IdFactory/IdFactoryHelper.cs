namespace Utils_DotNet.IdFactory
{
    /// <summary>
    /// 主键生成器，采用雪花算法
    /// </summary>
    public class IdFactoryHelper
    {
        private SnowFlake _snowFlake;
        private static readonly IdFactoryHelper _instance = new IdFactoryHelper();

        private IdFactoryHelper()
        {
            _snowFlake = new SnowFlake(0, 0, 0);
        }

        public static IdFactoryHelper Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 获取一个Id
        /// </summary>
        /// <returns>Id</returns>
        public long NextId()
        {
            return _snowFlake.NextId();
        }
    }
}
