using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SubSonic.SqlGeneration.Schema;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 实体类基类
    /// </summary>
    public abstract class BaseModel
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string CreateUserNumber { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建IP
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string CreateIP { get; set; }

        /// <summary>
        /// 修改人ID
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string UpdateUserNumber { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 修改IP
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string UpdateIP { get; set; }

        /// <summary>
        /// 获取实体类对象的Json格式对象
        /// </summary>
        /// <returns></returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// 实体类基类
    /// </summary>
    public abstract class BaseModelShort
    {
        /// <summary>
        /// ID
        /// </summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [SubSonicStringLength(30), SubSonicNullString]
        public string CreateUserNumber { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建IP
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string CreateIP { get; set; }

        /// <summary>
        /// 获取实体类对象的Json格式对象
        /// </summary>
        /// <returns></returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// 定位实体类基类
    /// </summary>
    public abstract class AddressBaseModel
    {
        /// <summary>
        /// 省份名称
        /// </summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string Province { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string City { get; set; }

        /// <summary>
        /// 地区名称
        /// </summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string District { get; set; }

        /// <summary>
        /// 街道名称
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string Street { get; set; }

        /// <summary>
        /// 具体定位
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string DetailName { get; set; }

        /// <summary>
        /// 城市编码
        /// </summary>
        [SubSonicStringLength(20), SubSonicNullString]
        public string CityCode { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
    }
}
