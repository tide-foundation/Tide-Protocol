// Tide Protocol - Infrastructure for the Personal Data economy
// Tide Protocol - Infrastructure for the Personal Data economy
// Copyright (C) 2019 Tide Foundation Ltd
// 
// This program is free software and is subject to the terms of 
// the Tide Community Open Source License as published by the 
// Tide Foundation Limited. You may modify it and redistribute 
// it in accordance with and subject to the terms of that License.
// This program is distributed WITHOUT WARRANTY of any kind, 
// including without any implied warranty of MERCHANTABILITY or 
// FITNESS FOR A PARTICULAR PURPOSE.
// See the Tide Community Open Source License for more details.
// You should have received a copy of the Tide Community Open 
// Source License along with this program.
// If not, see https://tide.org/licenses_tcosl-1-0-en

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public interface IKeyIdManager
    {
        Task<bool> Exist(Guid id);
        Task<KeyIdVault> GetById(Guid id);
        Task<List<KeyIdVault>> GetAll();
        Task<TideResponse> SetOrUpdate(KeyIdVault entity);
        Task<bool> Delete(Guid id);
    }
}