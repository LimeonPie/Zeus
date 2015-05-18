using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Zeus.Engine;

namespace Zeus.Helpers
{
	// Класс для проверки правильности формата
	// Пишется на работе на макосе в сублайме
	// 14.05.2015 Утро

	public enum VALIDATION_TYPE 
	{
		LONGITUDE,
		LATITUDE,
		CONCENTRATION,
		HEIGHT
	};

	public static class Validator 
	{
		public static bool validateItemForType(object item, VALIDATION_TYPE valType) {
            // Жуткий костыль, но тут какая-то загадочность с переменными
			switch (valType) {
				case VALIDATION_TYPE.LONGITUDE:
					double inspected = (double)item;
					if (inspected >= -180 && inspected <= 180) return true;
					break;
				case VALIDATION_TYPE.LATITUDE:
					var inspected1 = (double)item;
					if (inspected1 >= -90 && inspected1 <= 90) return true;
					break;
				case VALIDATION_TYPE.CONCENTRATION:
					double inspected2 = (double)item;
					if (inspected2 >= 0) return true;
					break;
				case VALIDATION_TYPE.HEIGHT:
					double inspected3 = (double)item;
					if (inspected3 >= 0) return true;
					break;
				default:
				  	break;
			}
			return false;
		}
	}
}