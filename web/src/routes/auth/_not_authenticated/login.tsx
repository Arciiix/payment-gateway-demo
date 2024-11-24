import { createFileRoute } from '@tanstack/react-router'
import { LoginForm } from '@/components/auth/login/LoginForm'

export const Route = createFileRoute('/auth/_not_authenticated/login')({
  component: RouteComponent,
})

function RouteComponent() {
  return (
    <div className="flex h-screen w-full items-center justify-center px-4">
      <LoginForm />
    </div>
  )
}
