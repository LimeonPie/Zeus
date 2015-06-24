using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Zeus.Engine;

namespace Zeus.Helpers
{

    // Класс для проверки правильности значений
    // Дипломная работа
    // ИВТ(б)-411 Миняев Илья
	// Класс для проверки правильности формата
	// Пишется на работе на макосе в сублайме с головной болью
	// 14.05.2015 Утро

	public enum VALIDATION_TYPE 
	{
		LONGITUDE,
		LATITUDE,
		CONCENTRATION,
        VELOCITY,
		HEIGHT,
        TIME,
        DELTA,
	};

	public static class Validator 
	{
		public static bool validateItemForType(double item, VALIDATION_TYPE valType) {
            // Жуткий костыль, но тут какая-то загадочность с переменными
			switch (valType) {
				case VALIDATION_TYPE.LONGITUDE:
                    if (item >= -180 && item <= 180) return true;
					break;
				case VALIDATION_TYPE.LATITUDE:
                    if (item >= -90 && item <= 90) return true;
					break;
				case VALIDATION_TYPE.CONCENTRATION:
                    if (item >= 0) return true;
					break;
				case VALIDATION_TYPE.HEIGHT:
                    if (item >= 0) return true;
					break;
                case VALIDATION_TYPE.TIME:
                    if (item > 0) return true;
                    break;
                case VALIDATION_TYPE.DELTA:
                    if (item > 0) return true;
                    break;
				default:
				  	break;
			}
			return false;
		}
	}
}