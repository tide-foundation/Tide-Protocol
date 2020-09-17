<template>
  <div id="rules-content">
    <h1>Rules</h1>
    <table>
      <tr>
        <th>Rule ID</th>
        <th>Name</th>
        <th>Tag</th>
        <th>Destination</th>
        <th>Action</th>
        <th>Automation</th>
        <th>State</th>
        <th>Expiration</th>
      </tr>

      <tr v-for="rule in rules" :key="rule.id" @click="$parent.selectedRule = rule">
        <td>{{ rule.id }}</td>
        <td>{{ rule.name }}</td>
        <td>{{ rule.tag }}</td>
        <td>{{ rule.destination }}</td>
        <td>{{ rule.action }}</td>
        <td>{{ rule.automation }}</td>
        <td>{{ rule.state }}</td>
        <td>{{ rule.expiration | moment("dddd, MMMM Do YYYY")}}</td>
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
}
</style>