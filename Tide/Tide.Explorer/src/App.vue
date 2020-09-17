<template>
  <div id="app">
    <button :disabled="selectedBlock == null" @click="back">Back</button>
    <table v-if="selectedBlock == null">
      <tr v-show="selectedBlock == null">
        <th>Date</th>
        <th>Contract</th>
        <th>Table</th>
        <th>Scope</th>
        <th>Index</th>
      </tr>

      <tr v-show="selectedBlock == null" v-for="block in activeBlockList" :key="block.id" @click="clickedRow(block.id)">
        <td>{{ block.dateCreated | date }}</td>
        <td>{{ block.contract }}</td>
        <td>{{ block.table }}</td>
        <td>{{ block.scope }}</td>
        <td>{{ block.index }}</td>
      </tr>
    </table>

    <table v-if="selectedBlock != null">
      <tr v-show="selectedBlock != null">
        <th>Data</th>
      </tr>

      <tr v-show="selectedBlock != null" v-for="block in blockHistory" :key="block.id">
        <td>{{ block.data }}</td>
      </tr>
    </table>
  </div>
</template>

<script>
import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";
import moment from "moment";
const contractNames = ["Unset", "Authentication"];
const tableNames = ["Unset", "Vault", "Users"];

export default {
    name: "App",
    components: {},
    data: function () {
        return {
            connection: null,
            blocks: [],
            selectedBlock: null,
        };
    },
    computed: {
        activeBlockList: function () {
            return [].concat(this.blocks.filter((b) => !b.stale)).reverse();
        },
        blockHistory: function () {
            if (this.selectedBlock == null) return null;
            return this.blocks.filter((b) => b.contract == this.selectedBlock.contract && b.table == this.selectedBlock.table && b.scope == this.selectedBlock.scope && b.index == this.selectedBlock.index);
        },
    },
    filters: {
        contractName: (v) => contractNames[v],
        tableName: (v) => tableNames[v],
        date: (v) => moment(String(v)).format("DD/MM/YYYY hh:mm"),
    },
    created() {
        const base = this;
        this.connection = new HubConnectionBuilder().withUrl("https://tidesimulator.azurewebsites.net/hub").configureLogging(LogLevel.Information).build();

        this.connection.on("NewBlock", function (block) {
            block.new = true;
            base.blocks.push(block);
        });

        this.connection.on("Populate", function (blocks) {
            base.blocks = blocks;
        });

        this.connection
            .start({ withCredentials: true })
            .then(function () {
                base.connection.invoke("Populate");
            })
            .catch(function (err) {
                return console.error(err.toString());
            });
    },
    methods: {
        clickedRow(blockId) {
            this.selectedBlock = this.blocks.find((b) => b.id == blockId);
        },
        back() {
            this.selectedBlock = null;
        },
    },
};
</script>

<style lang="scss">
@import url("https://fonts.googleapis.com/css2?family=Montserrat:wght@300&display=swap");
#app {
    width: 1000px;
    font-family: "Montserrat", sans-serif;
}

table {
    width: 100%;
}

th {
    text-align: left;
}

td,
th {
    border: 1px solid #dddddd;
    text-align: left;
    padding: 8px;
}

tr:nth-child(even) {
    background-color: #dddddd;
}

tr {
    cursor: pointer;
    &:hover {
        background-color: #fcf6c0;
    }

    .new {
        background-color: yellow !important;
    }
}
</style>
