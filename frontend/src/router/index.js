import { createRouter, createWebHistory } from 'vue-router'
import MainLayout from '../layouts/MainLayout.vue'
import SalaryCompositionList from '../views/salary-composition/SalaryCompositionList.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: MainLayout,
      children: [
        {
          path: '', 
          name: 'SalaryComposition',
          component: SalaryCompositionList
        }
      ]
    }
  ]
})

export default router