import { LikeMember } from "./likeMember"
import { Photo } from "./photo"

export interface Member extends LikeMember {
  created: Date
  lastActive: Date
  gender: string
  introduction: string
  lookingFor: string
  interests: string
  country: string
  photos: Photo[]
}


