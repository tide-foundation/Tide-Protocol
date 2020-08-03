<template>
  <section id="thanks-container">
    <div>
      Your application has been successfully submitted
      <a
        class="btn-default show-details  pointer"
        @click="$bus.$emit('update-menu', { name: 'Home', route: '/' })"
        >Return home<i class="fa fa-angle-right"></i
      ></a>
      <a
        class="btn-default show-details  pointer"
        @click="
          $bus.$emit('update-menu', { name: 'Dashboard', route: '/dashboard' })
        "
        >My dashboard<i class="fa fa-angle-right"></i
      ></a>
      <img id="new-home" src="../assets/img/slide2.jpg" alt="Your new home" />
    </div>
  </section>
</template>

<script>
export default {
  created() {
    this.$loading(false, "");
  }
};
</script>

<style lang="scss" scoped>
#thanks-container {
  height: calc(100vh - 130px);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 30px;
  text-align: center;

  div {
    width: 90%;
    max-width: 700px;

    span {
      font-size: 20px;
      span {
        cursor: pointer;
        color: orange;
      }
      span:hover {
        color: #005ce3;
      }
    }
  }
}

.btn-default {
  margin: 20px 30px;
}

#new-home {
  margin-top: 50px;
  width: 550px;
  border: 2px solid black;
  box-shadow: 0px 0px 14px 0px rgba(0, 0, 0, 0.75);
}
</style>
