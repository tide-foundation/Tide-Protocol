<template>
  <div id="rules">
    <div id="rules-topbar">
      <div v-if="selectedRule != null">
        <button @click="deselectRule">BACK TO RULES</button>
      </div>
      <div v-if="selectedRule == null">
        <button @click="newRule">NEW RULE</button>
      </div>
    </div>
    <RuleList v-if="selectedRule == null" :rules="rules"></RuleList>
    <Rule v-if="selectedRule != null" :rule="selectedRule"></Rule>
  </div>
</template>

<script>
import RuleList from "./RuleList.vue";
import Rule from "./Rule.vue";
export default {
    components: { RuleList, Rule },
    data: function() {
        return {
            rules: [],
            seededIndex: 0,
            selectedRule: null
        };
    },
    created() {
        this.fetch();
    },
    methods: {
        async fetch() {
            this.$loading(true, "Fetching rules...");
            try {
                var rules = await this.$tide.getRules();
                console.log(rules);
                rules.forEach(rule => {
                    rule.condition = JSON.parse(rule.condition);
                    if (typeof rule.condition == "object") {
                        for (let i = 0; i < rule.condition.length; i++) {
                            const condition = rule.condition[i];
                            condition.index = i;
                            condition.selected = false;
                        }
                    }
                });
                this.rules = rules;
            } catch (thrownError) {
                console.log(thrownError);
                this.$bus.$emit("show-error", "Failed saving rule... :'(");
            } finally {
                this.$loading(false, "");
            }
        },
        deselectRule() {
            this.selectedRule = null;
            this.fetch();
        },
        newRule() {
            this.rules.push({
                id: this.$helper.generateUniqueId(),
                name: "My new rule",
                tag: "Address",
                destination: "Vendor",
                action: "Allow",
                automation: true,
                state: "Active",
                expiration: new Date(),
                conditions: []
            });
        },
        seedRules() {
            this.rules = [
                {
                    id: this.$helper.generateUniqueId(),
                    name: "My rule",
                    tag: "Email",
                    destination: "Vendor",
                    action: "Allow",
                    automation: true,
                    state: "Active",
                    expiration: new Date(),
                    conditions: [
                        // {
                        //     id: this.$helper.generateUniqueId(),
                        //     index: this.seededIndex++,
                        //     selected: false,
                        //     union: "And",
                        //     field: null,
                        //     operator: "=",
                        //     value: "field2",
                        //     level: 0
                        // },
                        // {
                        //     id: this.$helper.generateUniqueId(),
                        //     index: this.seededIndex++,
                        //     selected: false,
                        //     union: "Or",
                        //     field: null,
                        //     operator: ">",
                        //     value: "field3",
                        //     level: 0
                        // },
                        // {
                        //     id: this.$helper.generateUniqueId(),
                        //     index: this.seededIndex++,
                        //     selected: false,
                        //     union: "Or",
                        //     field: null,
                        //     operator: ">",
                        //     value: "field3",
                        //     level: 0
                        // },
                        // {
                        //     id: this.$helper.generateUniqueId(),
                        //     index: this.seededIndex++,
                        //     selected: false,
                        //     union: "And",
                        //     field: null,
                        //     operator: "=",
                        //     value: "field2",
                        //     level: 0
                        // },
                        // {
                        //     id: this.$helper.generateUniqueId(),
                        //     index: this.seededIndex++,
                        //     selected: false,
                        //     union: "And",
                        //     field: null,
                        //     operator: "=",
                        //     value: "field2",
                        //     level: 0
                        // },
                        // {
                        //     id: this.$helper.generateUniqueId(),
                        //     index: this.seededIndex++,
                        //     selected: false,
                        //     union: "And",
                        //     field: null,
                        //     operator: "=",
                        //     value: "field2",
                        //     level: 0
                        // }
                    ]
                }
            ];
        }
    }
};
</script>

<style lang="scss" scoped>
#rules {
    width: 100%;
    padding: 10px;
    #rules-topbar {
        width: 100%;
        margin-bottom: 20px;
    }
}
</style>