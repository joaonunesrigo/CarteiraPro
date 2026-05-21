import { QueryClientProvider } from '@tanstack/react-query'
import { ConfirmDialogProvider } from './components/ConfirmDialog'
import { ToastProvider } from './components/Toast'
import { queryClient } from './lib/queryClient'
import Dashboard from './pages/Dashboard'

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ConfirmDialogProvider>
        <ToastProvider>
          <Dashboard />
        </ToastProvider>
      </ConfirmDialogProvider>
    </QueryClientProvider>
  )
}
