using carEVA.Models;
using carEVA.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carEVA.Test
{

    [TestFixture]
    public class organizationUtilsTest
    {
        private carEVAContext db = new carEVAContext();
        [Test]
        public void return_default_area_if_notExists()
        {
            //Arrange
            string areaCode = "123123";
            //Act
            int result = organizationUtils.getOrganizationAreaIdFromCode(db, areaCode);
            //Assert
            Assert.AreEqual(result, 31, "review ID on database");
        }
        [Test]
        public void return_area_ID()
        {
            //Arrange
            string areaCode = "100";
            //Act
            int result = organizationUtils.getOrganizationAreaIdFromCode(db, areaCode);
            //Assert
            Assert.AreEqual(result, 3, "review ID on database");
        }
    }
}
