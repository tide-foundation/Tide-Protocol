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
    init(name vendor, uint64_t username) {
        require_auth(vendor);

        user_index users(get_self(), get_self().value);

        check(itr == users.end(), "That username already exists.");
    };

private:
    struct node
    {
        name ork_node;
        string ork_url;
        string ork_public;
    };

    struct [[eosio::table]] user
    {
        uint64_t id;
        name account;
        bool confirmed;
        std::vector<node> nodes;

        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"tideusers"_n, user> user_index;

    struct [[eosio::table]] fragment
    {
        uint64_t id;
        string public_key;
        string private_key_frag;
        string pass_hash;

        uint64_t primary_key() const { return id; }
    };
    typedef eosio::multi_index<"tidefrags"_n, fragment> frag_index;
};