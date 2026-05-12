import { BrowserRouter, Route, Routes } from 'react-router'
import { MainLayout } from '@/components/layout'
import { HomePage, NotFoundPage } from '@/pages'
import { ROUTES } from '@/constants/routes'

const GlobalRoute = () => (
  <BrowserRouter>
    <Routes>
      <Route element={<MainLayout />}>
        <Route path={ROUTES.HOME} element={<HomePage />} />
        <Route path={ROUTES.NOT_FOUND} element={<NotFoundPage />} />
      </Route>
    </Routes>
  </BrowserRouter>
)

export default GlobalRoute
