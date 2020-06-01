using System.Collections.Generic;
using Tide.Simulator.Models;

namespace Tide.Simulator.Classes {
    public class ContractLayer : IContractLayer {
        private readonly IBlockLayer _blockLayer;

        public ContractLayer(IBlockLayer blockLayer) {
            _blockLayer = blockLayer;
        }

        public SimulatorUser CreateUser(string username, string password) {
            var user = GetUser(username);
            if (user != null) {
                return null;
            }

            var tideUser = new SimulatorUser {
                OrkLinks = new List<string>()
            };

            if (_blockLayer.Write(Contract.Authentication, Table.Users, Consts.CONTRACT_HOLDER, username, tideUser)) {
                return tideUser;
            }

            return null;
        }


        public SimulatorUser GetUser(string username) {
            if (_blockLayer.Read<SimulatorUser>(Contract.Authentication, Table.Users, Consts.CONTRACT_HOLDER, username, out var user)) {
                return user;
            }

            return null;
        }

        //public bool AddFrag(AuthenticationModel model)
        //{
        //    var user = GetUser(model.Username);
        //    if (user == null) return false;

        //    if (user.OrkLinks.Contains(model.Ork)) return false;
        //    user.OrkLinks.Add(model.Ork);

        //    return _blockLayer.Write(Contract.Authentication, Table.Users, model.Username, user);
        //}
    }
}