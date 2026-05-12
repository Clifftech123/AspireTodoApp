import { AppProviders } from '@/providers'
import GlobalRoute from '@/router/GlobalRoute'

const App = () => (
  <AppProviders>
    <GlobalRoute />
  </AppProviders>
)

export default App
