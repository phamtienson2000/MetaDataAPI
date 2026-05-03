from dataclasses import dataclass, field
from typing import Optional, List


@dataclass
class MetadataAsset:
    uid: str
    type: str
    name: str
    description: Optional[str]      
    owner: Optional[str]         
    created_on: Optional[str]        
    updated_on: Optional[str]        
    tags: List[str] = field(default_factory=list)  # Always List, never None
