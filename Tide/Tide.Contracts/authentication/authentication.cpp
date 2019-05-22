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
#include <string>

using namespace eosio;
using std::string;
class[[eosio::contract("authentication")]] authentication : public eosio::contract
{

public:
    using contract::contract;

    authentication(name receiver, name code, datastream<const char *> ds) : contract(receiver, code, ds){};

    [[eosio::action]] void
    addork(name ork_node, uint64_t username, string public_key) {
        require_auth(ork_node);

        // Gather the ork list
        ork_index orks(get_self(), get_self().value);

        // Does the ork already exists?
        auto itr = orks.find(username);

        // It does not. Let's add it
        if (itr == orks.end())
        {
            orks.emplace(ork_node, [&](auto &t) {
                t.id = username;
                t.account = ork_node;
                t.public_key = public_key;
            });
        }
        else
        {
            // Otherwise, update the entry
            auto ork = *itr;
            check(ork.account == ork_node, "You do not have permission to alter this ork node.");

            orks.modify(itr, ork_node, [&](auto &t) {
                t.account = ork_node;
                t.public_key = public_key;
            });
        }
    };

    [[eosio::action]] void
    inituser(name vendor, uint64_t username, uint64_t time) {
        require_auth(vendor);

        // Ensure timeout does not equal 0. 0 == confirmed account
        check(time != 0, "Timeout can not be 0");

        // Get user table, scoped to the vendor.
        user_index users(get_self(), get_self().value);

        // Does the user already exists?
        auto itr = users.find(username);

        // It does not. Let's add it
        if (itr == users.end())
        {
            users.emplace(vendor, [&](auto &t) {
                t.id = username;
                t.timeout = time;
            });
        }
        else
        {
            // Otherwise, update the entry
            users.modify(itr, vendor, [&](auto &t) {
                t.timeout = time;
            });
        }
    };

    [[eosio::action]] void
    confirmuser(name vendor, uint64_t username) {
        require_auth(vendor);

        // Get user table, scoped to the vendor.
        user_index users(get_self(), get_self().value);

        // If the pending user does not exist, return.
        auto itr = users.find(username);
        check(itr != users.end(), "That username has not been initialized.");

        auto user = *itr;
        check(user.timeout != 0, "That user has already been confirmed.");

        // Otherwise, update the entry
        users.modify(itr, vendor, [&](auto &t) {
            t.timeout = 0;
        });
    };


private:
    struct node
    {
        name ork_node;
        string ork_url;
        string ork_public;
    };

    struct [[eosio::table]] ork
    {
        uint64_t id; // username
        name account;
        string url;
        string public_key;

        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"orks"_n, ork> ork_index;

    struct [[eosio::table]] user
    {
        uint64_t id;      // username
        uint64_t timeout; // unix. 0 == confirmed
        std::vector<node> nodes;

        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"users"_n, user> user_index;

    struct [[eosio::table]] fragment
    {
        uint64_t id;
        string public_key;
        string private_key_frag;
        string pass_hash;

        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"frags"_n, fragment> frag_index;
};