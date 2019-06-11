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

#include <eosio/eosio.hpp>
//#include "../lib.hpp"
#include <string>

using namespace eosio;
using std::string;
class[[eosio::contract("vendors")]] vendors : public eosio::contract
{

public:
    using contract::contract;

    vendors(name receiver, name code, datastream<const char *> ds) : contract(receiver, code, ds){};

    // Converts a Tide account into an vendor account which can onboard users under it's context.
    // The Tide account must have been made as a top-tier account using official means.
    [[eosio::action]] void
    addvendor(name payer, name account, uint64_t username, string public_key, string desc) {
        require_auth(payer);

        user_index users("xtidemasterx"_n, "xtidemasterx"_n.value);

        auto userItr = users.find(username);
        check(userItr != users.end(), "That user does not exists.");

        // Gather the vendor list
        vendor_index vendors(get_self(), get_self().value);

        // Does the vendor already exists?
        auto itr = vendors.find(username);

        // It does not. Let's add it
        if (itr == vendors.end())
        {
            vendors.emplace(payer, [&](auto &t) {
                t.id = username;
                t.account = account;
                t.public_key = public_key;
                t.desc = desc;
            });
        }
        else
        {
            // Otherwise, update the entry
            vendors.modify(itr, payer, [&](auto &t) {
                t.public_key = public_key;
                t.desc = desc;
            });
        }

        // Remove funds from payer here
    };

    // Converts a Tide account into an vendor account which can onboard users under it's context.
    [[eosio::action]] void
    adduser(uint64_t username, uint64_t vendor) {
        vendor_index vendors(get_self(), get_self().value);

        auto itr = vendors.find(vendor);
        check(itr != vendors.end(), "That vendor does not exists.");

        auto vendorObj = *itr;
        require_auth(vendorObj.account);

        vendors.modify(itr, vendorObj.account, [&](auto &t) {
            t.users.push_back(username);
        });
    };

private:
    struct orklink
    {
        uint64_t vendor;
        std::vector<uint64_t> ork_ids;
    };

    struct [[eosio::table]] user
    {
        uint64_t id;      // username
        name account;     // blockchain account
        uint64_t timeout; // unix. 0 == confirmed
        uint64_t vendor;  // The vendor the user went through to register
        std::vector<orklink> ork_links;

        uint64_t primary_key() const { return id; }
    };

    struct [[eosio::table]] vendor
    {
        uint64_t id; // Username, scoped to ork
        name account;
        string public_key;
        string desc;
        std::vector<uint64_t> users;

        uint64_t primary_key() const { return id; }
    };

    typedef eosio::multi_index<"vendors"_n, vendor> vendor_index;
    typedef eosio::multi_index<"tideusers"_n, user> user_index;
};
