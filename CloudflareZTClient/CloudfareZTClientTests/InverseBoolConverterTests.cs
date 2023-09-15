namespace CloudfareZTClientTests
{
    using System.Globalization;
    using CloudflareZTClient.Converters;
    using NUnit.Framework;

    /// <summary>
    /// Inverse Bool converter unit tests.
    /// Note: unit tests names describe what's tests are preforming to do.
    /// </summary>
    public class InverseBoolConverterTests
    {
        [Test]
        public void Convert_True_ReturnsFalse()
        {
            // Arrange
            var converter = new InverseBoolConverter();

            // Act
            var result = converter.Convert(true, typeof(bool), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.IsInstanceOf<bool>(result);
            Assert.IsFalse((bool)result);
        }

        [Test]
        public void Convert_False_ReturnsTrue()
        {
            // Arrange
            var converter = new InverseBoolConverter();

            // Act
            var result = converter.Convert(false, typeof(bool), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.IsInstanceOf<bool>(result);
            Assert.IsTrue((bool)result);
        }

        [Test]
        public void Convert_Back_ReturnsSameValue()
        {
            // Arrange
            var converter = new InverseBoolConverter();

            // Act
            var result = converter.ConvertBack(true, typeof(bool), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, (bool)result);
        }

    }
}
