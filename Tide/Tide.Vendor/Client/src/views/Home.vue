<template>
    <div id="home">
        <h1>THIS IS AN EXAMPLE VENDOR WEBSITE</h1>
        <h2>PLEASE LOGIN OR REGISTER</h2>
        <div id="btn-bar"><button @click="clickedProfile">My Profile</button> <span id="tide"></span></div>
    </div>
</template>

<script>
export default {
    name: "Home",
    methods: {
        clickedProfile() {
            console.log("Clicked profile");
        },
    },
};
</script>

<style lang="scss">
#home {
    display: flex;
    justify-content: center;
    flex-direction: column;
    text-align: center;

    #btn-bar {
        display: flex;
        justify-content: center;
        flex-direction: row;
        button {
            margin: 10px;
        }
    }
}
</style>
