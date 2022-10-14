using System;
using FluentAssertions;
using Moq;
using Tide.Core;
using Tide.Encryption.Ed;
using Xunit;

namespace Tide.Ork.Test
{
    public class TideJwtTest
    {
        [Fact]
        public void VerifyShouldReturnTrue()
        {
            var key = new Ed25519Key();

            var jwt = new TideJwt(Guid.NewGuid());
            jwt.Sign(key);

            var tknTag = TideJwt.Parse(jwt.ToString());
            var isValid = tknTag.Verify(key.GetPublic());

            jwt.IsTide.Should().BeTrue();
            isValid.Should().BeTrue();
        }

        [Fact]
        public void VerifyShouldReturnFalse()
        {
            var key = new Ed25519Key();
            var keyTag = new Ed25519Key();

            var jwt = new TideJwt(Guid.NewGuid());
            jwt.Sign(key);

            var tknTag = TideJwt.Parse(jwt.ToString());
            var isValid = tknTag.Verify(keyTag.GetPublic());

            isValid.Should().BeFalse();
        }

        [Fact]
        public void IsTideShouldReturnFalse()
        {
            var jwt = TideJwt.Parse("eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMn0.tyh-VfuzIxCyGYDlkBA7DfyjrqmSHu6pQ2hoZuFqUSLPNY2N0mpHb3nk5K17HWP_3cYHBw7AhHale5wky6-sVA");

            jwt.IsTide.Should().BeFalse();
        }

        [Theory]
        [InlineData("2020-02-01", "0001/1/1", "2020-02-01 00:00:01", "2020-03-01 00:00:00")]
        [InlineData("2020-02-01", "0001/1/1", "2020-01-01 00:00:00", "2020-01-31 23:59:59")]
        [InlineData("2021-01-02", "0001/1/1", "0001/1/1", "2021-01-02 01:00:01")]
        [InlineData("2020-02-02", "0001/1/1", "2020-02-01 22:59:59", "0001/1/1")]
        [InlineData("2020-02-02", "2020-02-01 22:59:59", "0001/1/1", "0001/1/1")]
        [InlineData("2021-01-01", "0001/1/1", "0001/1/1", "0001/1/1")]
        public void IsOnTime_ShouldBeFalse_WhenDatesAreNotInRange(DateTime now, DateTime issuedAt, DateTime validFrom, DateTime validTo)
        {
            var mock = new Mock<ISystemTime>();
            mock.Setup(tme => tme.UtcNow).Returns(now);
            SystemTime.Default = mock.Object;

            now = DateTime.SpecifyKind(now, DateTimeKind.Utc);
            issuedAt = DateTime.SpecifyKind(issuedAt, DateTimeKind.Utc);
            validFrom = DateTime.SpecifyKind(validFrom, DateTimeKind.Utc);
            validTo = DateTime.SpecifyKind(validTo, DateTimeKind.Utc);

            //
            var jwt = new TideJwt(Guid.Empty);
            jwt.IssuedAt = issuedAt;
            jwt.ValidFrom = validFrom;
            jwt.ValidTo = validTo;

            var onTime = jwt.IsOnTime();

            //
            onTime.Should().BeFalse();
        }

        [Theory]
        [InlineData("2020-02-01", "0001/1/1", "2020-01-01", "2021-01-01")]
        [InlineData("2021-01-02", "0001/1/1", "0001/1/1", "2021-01-02 01:00:00")]
        [InlineData("2020-02-02", "0001/1/1", "2020-02-01 23:00:00", "0001/1/1")]
        [InlineData("2020-02-02", "2020-02-01 23:00:00", "0001/1/1", "0001/1/1")]
        public void IsOnTime_ShouldBeTrue_WhenDatesAreNotInRange(DateTime now, DateTime issuedAt, DateTime validFrom, DateTime validTo)
        {
            var mock = new Mock<ISystemTime>();
            mock.Setup(tme => tme.UtcNow).Returns(now);
            SystemTime.Default = mock.Object;

            now = DateTime.SpecifyKind(now, DateTimeKind.Utc);
            issuedAt = DateTime.SpecifyKind(issuedAt, DateTimeKind.Utc);
            validFrom = DateTime.SpecifyKind(validFrom, DateTimeKind.Utc);
            validTo = DateTime.SpecifyKind(validTo, DateTimeKind.Utc);

            //
            var jwt = new TideJwt(Guid.Empty);
            jwt.IssuedAt = issuedAt;
            jwt.ValidFrom = validFrom;
            jwt.ValidTo = validTo;

            var onTime = jwt.IsOnTime();

            //
            onTime.Should().BeTrue();
        }
    }
}
