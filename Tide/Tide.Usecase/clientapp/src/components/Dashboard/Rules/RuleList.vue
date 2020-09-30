<template>
    <div id="rules-content">
        <h1>Rules</h1>
        <table>
            <tr>
                <th>Rule ID</th>
                <th class="disabled">Name</th>
                <th>Tag</th>
                <th class="disabled">Destination</th>
                <th>Action</th>

                <th class="disabled">State</th>
                <th class="disabled">Expiration</th>
                <th>Override</th>
            </tr>

            <tr v-for="rule in rules" :key="rule.id" @click="$parent.selectedRule = rule">
                <td>{{ rule.ruleId }}</td>
                <td>{{ rule.name }}</td>
                <td>{{ rule.tag }}</td>
                <td>{{ rule.destination }}</td>
                <td>{{ rule.action }}</td>

                <td>{{ rule.state }}</td>
                <td>{{ rule.expiration | moment("dddd, MMMM Do YYYY") }}</td>
                <td>{{ typeof rule.condition == "boolean" }}</td>
            </tr>
        </table>
    </div>
</template>

<script>
export default {
    props: ["rules"],
    data: function() {
        return {};
    }
};
</script>

<style lang="scss" scoped>
#rules-content {
    width: 100%;

    .disabled {
        color: lightslategray;
        opacity: 0.3;
    }
}
</style>



