namespace MvcMovie.Utils
{
    /// <summary>
    /// SnowFlake 
    /// </summary>
    public class SnowFlake
    {
        /**
         * 起始的时间戳
         */
        private const long START_STMP = 1577808000000L;// 2020-01-01 00:00:00 Asia.Shanghai UTC+8 ms

        private const int SEQUENCE_BIT = 12; //序列号占用的位数
        private const int MACHINE_BIT = 5;   //机器标识占用的位数
        private const int DATACENTER_BIT = 5;//数据中心占用的位数

        /**
         * 每一部分的最大值
         */
        private const long MAX_DATACENTER_NUM = -1L ^ (-1L << DATACENTER_BIT);// 0-31
        private const long MAX_MACHINE_NUM = -1L ^ (-1L << MACHINE_BIT);      // 0-31
        private const long MAX_SEQUENCE = -1L ^ (-1L << SEQUENCE_BIT);        // 0-4095

        /**
         * 每一部分向左的位移
         */
        private const int MACHINE_LEFT = SEQUENCE_BIT; // 12
        private const int DATACENTER_LEFT = SEQUENCE_BIT + MACHINE_BIT; // 17
        private const int TIMESTMP_LEFT = DATACENTER_LEFT + DATACENTER_BIT;// 22

        /**
         * 工作机器 ID 中 DataCenterId 在高5位，MachineId 在第5位
         */
        public long DataCenterId { get; protected set; } //数据中心
        public long MachineId { get; protected set; }    //机器标识

        public long _sequence = 0L; // 初始序列号

        public long _lastStmp = -1L;// 上一次时间戳

        /**
         * 当前ID
         */
        public long CurrentId { get; private set; } // 当前生成的ID
        private object _lock = new();               // 锁对象应为私有且为引用类型


        /// <summary>
        /// 构造函数, dataCenterId[0,31], machineId[0,31]
        /// </summary>
        /// <param name="dataCenterId">数据中心Id[0,31]</param>
        /// <param name="machineId">机器Id[0,31]</param>
        /// <exception cref="ArgumentException"></exception>
        public SnowFlake(long dataCenterId, long machineId)
        {
            if (dataCenterId > MAX_DATACENTER_NUM || dataCenterId < 0)
            {
                throw new ArgumentException("datacenterId can't be greater than MAX_DATACENTER_NUM or less than 0");
            }
            if (machineId > MAX_MACHINE_NUM || machineId < 0)
            {
                throw new ArgumentException("machineId can't be greater than MAX_MACHINE_NUM or less than 0");
            }

            this.DataCenterId = dataCenterId;
            this.MachineId = machineId;
        }

        /// <summary>
        /// 构造函数, dataCenterId[0,31], machineId[0,31], sequence[0,4095]
        /// </summary>
        /// <param name="dataCenterId">数据中心Id[0,31]</param>
        /// <param name="machineId">机器Id[0,31]</param>
        /// <param name="sequence">初始序列号[0, 4095]</param>
        /// <exception cref="ArgumentException"></exception>
        public SnowFlake(long dataCenterId, long machineId, long sequence) : this(dataCenterId, machineId)
        {
            if (sequence > MAX_SEQUENCE || sequence < 0)
            {
                throw new ArgumentException("sequence can't be greater than MAX_SEQUENCE or less than 0");
            }
            this._sequence = sequence;
        }

        /**
         * 产生下一个ID
         *
         * @return SnowflakeId
         */
        public long NextId()
        {
            lock (_lock)
            {
                long currStmp = GetNewstmp();
                if (currStmp < _lastStmp)
                { 
                    // 时钟回拨
                    throw new Exception(
                        $"Clock moved backwards or wrapped around. Refusing to generate id for {_lastStmp - currStmp} ticks!");
                }
                if (currStmp == _lastStmp)
                { 
                    // 相同毫秒内，序列号自增
                    _sequence = (_sequence + 1) & MAX_SEQUENCE;
                    // 同一毫秒的序列数已经达到最大
                    if (_sequence == 0)
                    { 
                        currStmp = GetNextMill();
                    }
                }
                else
                {
                    // 不同毫秒内，序列号置为0或1
                    _sequence = currStmp & 1;
                }
                _lastStmp = currStmp;
                CurrentId = (currStmp - START_STMP) << TIMESTMP_LEFT //时间戳部分
                        | DataCenterId << DATACENTER_LEFT       //数据中心部分
                        | MachineId << MACHINE_LEFT             //机器标识部分
                        | _sequence;                            //序列号部分
                return CurrentId;
            }
        }

        /// <summary>
        /// 获取下一毫秒时间戳
        /// </summary>
        /// <returns></returns>
        private long GetNextMill()
        {
            long mill = GetNewstmp();
            while (mill <= _lastStmp)
            {
                mill = GetNewstmp();
            }
            return mill;
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        private long GetNewstmp()
        {
            return (DateTimeOffset.Now).ToUnixTimeMilliseconds();
        }
    }
}
