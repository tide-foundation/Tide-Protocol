<template>
  <div class="page-head ">
    <div class="breadcrumb-wrapper">
      <div class="container">
        <nav>
          <ol class="breadcrumb">

            <li class="active">
              <slot></slot>
            </li>
          </ol>
        </nav>
      </div>
    </div>
    <!-- .breadcrumb-wrapper -->
  </div>
</template>

<script>
export default {

}
</script>

<style>
.page-head {
  background: url("../assets/img/banner.jpg") #494c53 no-repeat center top;
  background-size: cover;
}
</style>
