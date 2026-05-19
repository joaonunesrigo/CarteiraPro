import { useState } from 'react'

export function useTabs(abaInicial) {
  const [abaAtiva, setAbaAtiva] = useState(abaInicial)

  function mudarAba(id) {
    setAbaAtiva(id)
  }

  return { abaAtiva, mudarAba }
}
