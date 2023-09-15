namespace CloudfareZTClientTests
{
    using System;
    using System.Threading.Tasks;
    using CloudflareZTClient.Models;
    using CloudflareZTClient.PageModels;
    using CloudflareZTClient.Services.Interfaces;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Nunit tests for the main page.
    /// Note: unit tests names describe what's tests are preforming to do.
    /// </summary>
    public class MainPageModelTest
    {
#region CheckVPNStatus tests
        [Test]
        public async Task CheckVPNStatus_ConnectedStatus_SetsIsConnectedToVPNTrue()
        {
            // Arrange
            var vpnConnectionServiceMock = new Mock<IVPNConnectionService>();
            vpnConnectionServiceMock.Setup(x => x.CheckStatusAsync())
                .ReturnsAsync(new StatusModel { status = "success", data = new DaemonDataModel { daemon_status = "connected" } });

            var mainPageModel = new MainPageModel(vpnConnectionServiceMock.Object);

            // Act
            await mainPageModel.CheckVPNStatusAsync();

            // Assert
            Assert.IsTrue(mainPageModel.IsConnectedToVPN);
        }

        [Test]
        public async Task CheckVPNStatus_ErrorStatus_SetsIsConnectedToVPNFalse()
        {
            // Arrange
            var vpnConnectionServiceMock = new Mock<IVPNConnectionService>();
            vpnConnectionServiceMock.Setup(x => x.CheckStatusAsync())
                .ReturnsAsync(new StatusModel { status = "error", message = "Error message" });

            var mainPageModel = new MainPageModel(vpnConnectionServiceMock.Object);

            // Act
            await mainPageModel.CheckVPNStatusAsync();

            // Assert
            Assert.IsFalse(mainPageModel.IsConnectedToVPN);
        }

        [Test]
        public async Task CheckVPNStatus_ErrorStatus_SetsVPNStatusMessageError()
        {
            // Arrange
            var vpnConnectionServiceMock = new Mock<IVPNConnectionService>();
            vpnConnectionServiceMock.Setup(x => x.CheckStatusAsync())
                .ReturnsAsync(new StatusModel { status = "error", message = "Error message" });

            var mainPageModel = new MainPageModel(vpnConnectionServiceMock.Object);

            // Act
            await mainPageModel.CheckVPNStatusAsync();

            // Assert
            Assert.AreEqual("Error message", mainPageModel.VPNStatus);
        }

        [Test]
        public async Task CheckVPNStatus_ConnectedStatus_SetsVPNStatusConnected()
        {
            // Arrange
            var vpnConnectionServiceMock = new Mock<IVPNConnectionService>();
            vpnConnectionServiceMock.Setup(x => x.CheckStatusAsync())
                .ReturnsAsync(new StatusModel { status = "success", data = new DaemonDataModel { daemon_status = "connected" } });

            var mainPageModel = new MainPageModel(vpnConnectionServiceMock.Object);

            // Act
            await mainPageModel.CheckVPNStatusAsync();

            // Assert
            Assert.AreEqual("connected", mainPageModel.VPNStatus);
        }

        [Test]
        public async Task CheckVPNStatus_ErrorStatus_DoesNotThrowNullReferenceException()
        {
            // Arrange
            var vpnConnectionServiceMock = new Mock<IVPNConnectionService>();
            vpnConnectionServiceMock.Setup(x => x.CheckStatusAsync())
                .ReturnsAsync(new StatusModel { status = "error", message = "Error message" });

            var mainPageModel = new MainPageModel(vpnConnectionServiceMock.Object);

            // Act & Assert
            try
            {
                await mainPageModel.CheckVPNStatusAsync();
            }
            catch (NullReferenceException)
            {
                // Fail the test if a NullReferenceException is thrown
                Assert.Fail("NullReferenceException was thrown.");
            }
        }
        [Test]
        public async Task CheckVPNStatus_SuccessStatus__NoData_DoesNotThrowNullReferenceException()
        {
            // Arrange
            var vpnConnectionServiceMock = new Mock<IVPNConnectionService>();
            vpnConnectionServiceMock.Setup(x => x.CheckStatusAsync())
                .ReturnsAsync(new StatusModel { status = "success" });

            var mainPageModel = new MainPageModel(vpnConnectionServiceMock.Object);

            // Act & Assert
            try
            {
                await mainPageModel.CheckVPNStatusAsync();
            }
            catch (NullReferenceException)
            {
                // Fail the test if a NullReferenceException is thrown
                Assert.Fail("NullReferenceException was thrown.");
            }
        }
        #endregion
        #region ConnectToVPNAsync Tests
        [Test]
        public async Task ConnectToVPNAsync_ConnectSuccess_SetsConnectedState()
        {
            // Arrange
            var vpnConnectionServiceMock = new Mock<IVPNConnectionService>();

            vpnConnectionServiceMock.Setup(x => x.ConnectToVpnAsync())
                .ReturnsAsync(new StatusModel { status = "success", data = new DaemonDataModel { daemon_status = "connected" } });
           
            var mainPageModel = new MainPageModel(vpnConnectionServiceMock.Object);

            // Act
            await mainPageModel.ConnectToVPNAsync(true);

            // Assert
            Assert.IsTrue(mainPageModel.IsConnectedToVPN);
            Assert.AreEqual("connected", mainPageModel.VPNStatus);
        }

        [Test]
        public async Task ConnectToVPNAsync_DisconnectSuccess_SetsDisconnectedState()
        {
            // Arrange
            var vpnConnectionServiceMock = new Mock<IVPNConnectionService>();

            vpnConnectionServiceMock.Setup(x => x.DisconnectVpnAsync())
                .ReturnsAsync(new StatusModel { status = "success", data = new DaemonDataModel { daemon_status = "disconnected" } });
            
            var mainPageModel = new MainPageModel(vpnConnectionServiceMock.Object);

            // Act
            await mainPageModel.ConnectToVPNAsync(false);

            // Assert
            Assert.IsFalse(mainPageModel.IsConnectedToVPN);
            Assert.AreEqual("disconnected", mainPageModel.VPNStatus);
        }
        [Test]
        public async Task ConnectToVPNAsync_ErrorStatus_SetsErrorMessage()
        {
            // Arrange
            var vpnConnectionServiceMock = new Mock<IVPNConnectionService>();

            vpnConnectionServiceMock.Setup(x => x.ConnectToVpnAsync())
                .ReturnsAsync(new StatusModel { status = "error", message = "Error message" });
           
            var mainPageModel = new MainPageModel(vpnConnectionServiceMock.Object);

            // Act
            await mainPageModel.ConnectToVPNAsync(true);

            // Assert
            Assert.IsFalse(mainPageModel.IsConnectedToVPN);
            Assert.AreEqual("Error message", mainPageModel.VPNStatus);
        }

        [Test]
        public async Task ConnectToVPNAsync_NullDaemonStatus_SetsConnectionError()
        {
            // Arrange
            var vpnConnectionServiceMock = new Mock<IVPNConnectionService>();
           
            vpnConnectionServiceMock.Setup(x => x.ConnectToVpnAsync())
                .ReturnsAsync((StatusModel)null); // Simulate a null daemonStatus
            
            var mainPageModel = new MainPageModel(vpnConnectionServiceMock.Object);

            // Act
            await mainPageModel.ConnectToVPNAsync(true);

            // Assert
            Assert.IsFalse(mainPageModel.IsConnectedToVPN);
            Assert.AreEqual("Connection error", mainPageModel.VPNStatus);
        }
        #endregion
    }
}
