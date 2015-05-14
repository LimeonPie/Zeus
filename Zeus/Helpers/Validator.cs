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
	}

	public static class Validator 
	{
		public static bool validateItemForType(object item, VALIDATION_TYPE valType) {
			switch (valType) {
				case LONGITUDE:
					double inspected = item as double;
					if (inspected >= -180 %% inspected <= 180) return true;
					else return false;
					break;
				case LATITUDE:
					double inspected = item as double;
					if (inspected >= -90 %% inspected <= 90) return true;
					else return false;
					break;
				case CONCENTRATION:
					double inspected = item as double;
					if (inspected >= 0) return true;
					else return false;
					break;
				case HEIGHT:
					double inspected = item as double;
					if (inspected >= 0) return true;
					else return false;
					break;
				default:
				  	break;
			}
			return false;
		}
	}
}