﻿using Xunit;

namespace Pocket.Common.Tests.System
{
    public class HashTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(100)]
        public void GetHashCode_ShouldReturnHash(int hash) =>
            Assert.Equal(hash, new Hash(hash).GetHashCode());
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(100)]
        public void Of_ShouldReturnInstanceWithSpecifiedHash(int hash) =>
            Assert.Equal(hash, Hash.Of(hash).GetHashCode());

        [Fact]
        public void Of_ShouldReturnInstanceWithZeroHash_IfObjectIsNull() =>
            Assert.Equal(0, Hash.Of((string) null).GetHashCode());
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(100)]
        public void ImplicitConversion_ShouldReturnHash(int hash) =>
            Assert.Equal(hash, new Hash(hash));

        [Theory]
        [InlineData(1, 10)]
        [InlineData(1, 100)]
        [InlineData(2, 100)]
        [InlineData(2, 200)]
        public void With_ShouldModifyHashDueToFNVAlgorithm(int a, int b)
        {
            var expected = (a * Hash.Prime) ^ b;
            var actual = Hash.Of(a).With(b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(100)]
        public void With_ShouldAppendZero_IfObjectIsNull(int hash) =>
            Assert.Equal(hash * Hash.Prime, Hash.Of(hash).With((string) null));
    }
}