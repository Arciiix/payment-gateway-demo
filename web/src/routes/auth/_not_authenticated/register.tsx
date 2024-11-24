import * as React from 'react'
import { createFileRoute } from '@tanstack/react-router'
import { RegisterForm } from '@/components/auth/register/RegisterForm'

export const Route = createFileRoute('/auth/_not_authenticated/register')({
  component: RouteComponent,
})

function RouteComponent() {
  return (
    <div className="flex h-screen w-full items-center justify-center px-4">
      <RegisterForm />
    </div>
  )
}
