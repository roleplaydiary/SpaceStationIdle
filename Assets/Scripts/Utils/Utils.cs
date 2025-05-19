using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Конвертирует большое число в строковое представление с сокращениями (k, m, b, t) без остатка, где это возможно.
    /// </summary>
    /// <param name="number">Число для конвертации.</param>
    /// <returns>Строковое представление числа с сокращением.</returns>
    public static string FormatBigNumber(double number)
    {
        if (number >= 1000000000000)
        {
            return (long)(number / 1000000000000f) + "t"; // Trillion
        }
        if (number >= 1000000000)
        {
            return (long)(number / 1000000000f) + "b"; // Billion
        }
        if (number >= 1000000)
        {
            return (long)(number / 1000000f) + "m"; // Million
        }
        if (number >= 1000)
        {
            return (long)(number / 1000f) + "k"; // Thousand
        }
        return ((long)number).ToString(); // Возвращаем целое число как строку
    }

    /// <summary>
    /// Перегрузка метода для работы с целочисленными значениями.
    /// </summary>
    /// <param name="number">Целое число для конвертации.</param>
    /// <returns>Строковое представление числа с сокращением.</returns>
    public static string FormatBigNumber(long number)
    {
        return FormatBigNumber((double)number);
    }

    /// <summary>
    /// Еще одна перегрузка для работы с float значениями.
    /// </summary>
    /// <param name="number">Вещественное число для конвертации.</param>
    /// <returns>Строковое представление числа с сокращением.</returns>
    public static string FormatBigNumber(float number)
    {
        return FormatBigNumber((double)number);
    }
}